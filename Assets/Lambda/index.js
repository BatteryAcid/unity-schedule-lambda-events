var aws = require('aws-sdk');

exports.handler = async(event, context, callback) => {

    console.log('Received event:', JSON.stringify(event, null, 2));

    const STATE_MACHINE_ARN = 'arn:aws:states:YOUR_REGION:YOUR_ACCOUNT:stateMachine:YOUR_STATE_MACHINE';

    if (event.headers != null) {
        // if source:unity header exists, request was from client unity app
        var sourceHeaderValue = event.headers["source"];
        if (sourceHeaderValue != null && sourceHeaderValue == "unity") {
            console.log(event.body);
            // call to execute the step function
            return startStepFunctionExecution(event.body);
        }
    }
    else {
        // otherwise, step function "build wait time" completed
        onStepFunctionComplete(event);
    }

    async function startStepFunctionExecution(requestBody) {
        console.log("setup step function");

        var buildTime = determineBuildTime(requestBody.buildId);
        const buildStartTime = new Date(new Date().getTime() + buildTime * 60000);
        console.log(buildStartTime);
        
        var jsonParsedBody = JSON.parse(requestBody);
        console.log(jsonParsedBody);

        var params = {
            stateMachineArn: STATE_MACHINE_ARN,
            input: JSON.stringify({ waitUntil: buildStartTime, buildId: jsonParsedBody.buildId }) // '2021-12-23T06:05:15Z' correct format example
        };

        var stepfunctions = new aws.StepFunctions();
        console.log("startExecution: " + params.input);

        const { executionArn } = await stepfunctions.startExecution(params).promise();

        const response = {
            statusCode: 200,
            body: JSON.stringify({
                message: 'Step function execution started'
            })
        };
        callback(null, response);
    }

    function onStepFunctionComplete(eventData) {
        console.log("step function complete...");

        // complete post execution actions here
        // e.g. save or update build status in datastore, like dynamodb

        const response = {
            statusCode: 200,
            body: eventData
        };
        callback(null, response);
    }
    
    function determineBuildTime(buildId) {
        var buildTime = 1; // 1 minute default
        
        // set build time based on id
        if (buildId != null) {
            if (buildId == "temple") {
                buildTime = 5; 
            }
        } 
        return buildTime;
    }
};
