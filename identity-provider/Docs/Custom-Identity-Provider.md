# Custom Identity Provider
Demo of an Identity Provider that follows the [documentation](https://learn.microsoft.com/en-us/azure/healthcare-apis/fhir/configure-identity-providers)

However, it doesn't work :(

## Prerequisites
For this to work you need to set up the following:
- Configure [Devtunnels](https://learn.microsoft.com/en-us/azure/developer/dev-tunnels/).
- Add `devtunnel=https://[TUNNELID]-7023.euw.devtunnels.ms` to 
  the `.env` file in this folder

Configure Authentication on the FHIR Service
- Add an Identity Provider with Authority equal to the devtunnel url
- Add an Application to the Identity Provider with:
  - Client ID: `smart-fhir-test-clientid`
  - Audience: `smart-fhir-test-audience`

Start the IdentityServer Project and run the scenario.

## Scenario 1: System Scope
### Get a token from the local Identity Provider
```http
# @name systemscope 
POST {{$dotenv devtunnel}}/connect/token
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
This will fail with `403`. This means that:
1. The Identity Provider is configured correctly. This is also proven by the
   fact that the call to `/.well-known/openid-configuration` is logged in the
   console window.
2. Somehow our token is malformed, but we follow the specification:
  - `iss` claim matches the OpenId Configuration and the Authority
  - `azp` claim is present and matches the Client ID setting
  - `aud` claim is present and matches the Audience setting
  - `scp` claim is present and is a valid SMART system scope

```http
GET {{$dotenv fhirserver}}/Patient
Authorization: Bearer {{token}}
```

## Scenario 2: User Scope
Since the documentation doesn't talk about system scopes, let's try a User
scope.

### Get a token from the local Identity Provider
```http
# @name userscope 
POST {{$dotenv devtunnel}}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=test
&client_secret=test
&scope=user/*.read
```

```http
@token = {{userscope.response.body.access_token}}
```

### Use it to access the FHIR Server
This also fails even if the `fhirUser` claim is present.

```http
GET {{$dotenv fhirserver}}/Patient
Authorization: Bearer {{token}}
```