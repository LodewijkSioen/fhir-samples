# Using the local OSS FHIR Server
For this demo you need to set up the following:

## Prerequisites
Clone the [Microsoft OSS Fhir Server][1]. Build the project. 
Add the following lines to the environmentVariables of the `sqlServer` 
profile in the `launchSettings.json` file of the `R4.Web` project:

```
"FhirServer:Security:Authentication:Audience": "smart-fhir-test-audience",
"FhirServer:Security:Authentication:Authority": "https://localhost:7023",
```

[1]: https://github.com/microsoft/fhir-server

## Scenario
### Get a token from entra id
```http
# @name systemscope 
POST https://localhost:7023/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=test
&client_secret=test
&scope=system/*.read
```

```http
@token = {{systemscope.response.body.access_token}}
```

### Use it to access the FHIR Server
This works. So apparently Azure has a different configuration than the OSS 
version.

```http
### GET Patient 
GET https://localhost:44348/Patient
Authorization: Bearer {{token}}
```