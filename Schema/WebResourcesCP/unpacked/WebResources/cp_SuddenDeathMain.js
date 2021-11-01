function OnLoad(executionContext)
{
	var formContext = executionContext.getFormContext();
	formContext.getControl("cp_approvalstatus").removeOption(778230000) //NA
	formContext.getControl("cp_approvalstatus").removeOption(778230002) //Rejected
	//remove approved (001) unless the user is in the supervisor role
	debugger;
	var currentUserRoles = Xrm.Utility.getGlobalContext().userSettings.securityRoles;
    var roleId = "3A7DDADF-D184-EB11-A812-000D3AB522EE".toLowerCase();
    var hasRole = false;
	for (var i = 0; i < currentUserRoles.length; i++)
	{
		var userRoleId = currentUserRoles[i];
		if (userRoleId == roleId)
		{
			
			hasRole = true;
		}
	}
	if (!hasRole)
    {
        formContext.getControl("cp_approvalstatus").removeOption(778230001) //Approved
    }

}

