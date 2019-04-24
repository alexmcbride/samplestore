# Sample Store
Azure service created as part of 4th year Cloud Platform Development module.

The app comprises three components:

* RESTful web service (WebApi) for adding, removing, and editing samples in Azure table storage
* WebForms application for uploading sound file and storing in Azure blob storage
* WebJob to re-sample these sound files down to 20 seconds and then update table storage
