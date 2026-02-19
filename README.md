# Safevault project

Hi there, here's a little resume on what I did (And what copilot told me according to the course requirements) for this module's project

### Activity 1
1. After finishing with the basic setup defined in the activity instructions, I started by changing up the form Webform to make use of the model and display the validation messages, this can be seen in [Webform.cshtml](./SafeVault.Web/Views/Forms/Webform.cshtml).
2. Server side validation and sanitization was implemented, according to copilot's suggestions
  - A new model for the form data was created in [WebformModel.cs](./SafeVault.Web/Models/WebformModel.cs) which defines minimal validations and required fields
  - Then the controller for the webform was defined in [WebformController.cs](./SafeVault.Web/Controllers/WebformController.cs)
3.  The implementation of parametrized queries can be seen in [AuthController.cs](./SafeVault.Web/Controllers/AuthController.cs)
4. Added a new endpoint for registering (This was for testing sake in order to see if the database worked properly)
5. Implemented unit tests using NUnit which can be found in [AuthControllerTests.cs](./SafeVault.Tests/TestInputValidation.cs) (They pass as of writing this in activity 1 (And this commit))
