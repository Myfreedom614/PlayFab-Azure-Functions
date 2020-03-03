# pf-af-testfuncs
Azure Functions called by PlayFab pf-main integration tests

This repository contains various Azure Functions that are called by PlayFab integration tests in the [pf-main](https://github.com/PlayFab/pf-main) repository that test the new Azure Functions based CloudScript feature. At the time of writing the specific tests are the ones found in [FunctionsTests.cs](https://github.com/PlayFab/pf-main/blob/master/Tests/Integration/ApiServer/FunctionsTests.cs)

The specific Azure Functions are;

* CheckFunctionExecutionContext
* GetSecureObject
* Identity
* IdentityArray
* IdentityArrayPlayStream
* IdentityArrayTask
* IdentityQueued
* IdentityQueuedPSV1
* IdentityQueuedPSV2
* IdentityQueuedTask
* IdentityPlayStream
* IdentityTask
* LongRunning
* ReturnProfile
* Throwing
* UpdateSecureObject