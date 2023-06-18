# Read all users

The "Read All Users" configuration allows you to retrieve information about all users.

## Configuration

To retrieve information about all users, make a GET request to the following endpoint pattern:

```regex
/(?:beta|v1\.0)/users
```

The configuration details are as follows:

```json
POST /v1/profiles/00000000-0000-0000-0000-000000000000/rules
{
    "name": "read-all-users",
    "displayName": "Read All Users",
    "Method": "GET",
    "Pattern": "^\\/(?:beta|v1\\.0)\\/users$",
    "Version": "1.0"
}
```

By making a GET request to the specified endpoint, you can retrieve information about all users.

### Advanced configuration

If you want more control over this rule, you can add a request configuration. For example, the following configuration disables the delta feature:

```json
POST /v1/profiles/00000000-0000-0000-0000-000000000000/rules
{
    "name": "read-all-users",
    "displayName": "Read All Users",
    "Method": "GET",
    "Pattern": "^\\/(?:beta|v1\\.0)\\/users$",
    "Request": {
        "MemberName": "DeltaToken",
        "Operator": "Equal",
        "TargetValue": false
    },
    "Version": "1.0"
}
```

With this configuration, the delta feature is disabled when making the GET request to retrieve information about all users.

> Another request configuration exists, please read the user entry filter configuration for additional options and customization.