# Using Entra ID token
For this demo you need to set up the following:

## Prerequisites
Create a new App Registration in Entra ID with the following settings:
- Authentication: Allow public client flow: No
- Certificates and Secrets: Add a secret and keep the value
- API Permissions: Azure HealthCare API:
  - user_impersonation
  - system.all.read

Give the application the FHIR SMART User role on the FHIR Service.

Next, add the following to the `.env` file in this folder:
```
clientid=YOUR APP REGISTRATION CLIENT ID
clientsecret=YOUR APP REGISTRATION SECRET
```

## Scenario
### Get a token from entra id
```http
# @name login 
POST https://login.microsoftonline.com/{{$dotenv tenantid}}/oauth2/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&resource={{$dotenv fhirserver}}
&client_id={{$dotenv clientid}}
&client_secret={{$dotenv clientsecret}}
```
```http
 Extract access token from getAADToken request
@token = {{login.response.body.access_token}}
```

### Use it to access the FHIR Server
```http
### GET Patient 
GET {{$dotenv fhirserver}}/Patient
Authorization: Bearer {{token}}
```