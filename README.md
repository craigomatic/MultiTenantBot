# MultiTenantBot
A simple example of creating a multi-tenant bot using the backchannel

## Usage
1. Publish the MultiTenantBot to http://dev.botframework.com
2. Enable the DirectLine channel and copy the token
3. Place the DirectLine token inside Index.cshtml of both the Tenant1 and Tenant2 projects
4. Run either of the TenantX projects and notice the returned message identifies the correct tenant
