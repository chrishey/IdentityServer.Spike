# IdentityServer.Spike
## Steps to get this working locally
Following the instructions here - [http://identityserver.github.io/Documentation/docs/overview/mvcGettingStarted.html](http://identityserver.github.io/Documentation/docs/overview/mvcGettingStarted.html)
* Create a self signed certificate in IIS
* Export the certificate and install it in your local computer -> Trusted People
* Set up an IIS site to use SSL and pick the create certificate, use the port that identity server is running on
* Add an entry to your host file for the machine name that IIS is using
* Navigate to the site https://*machine name*:*identity server port*/identity
