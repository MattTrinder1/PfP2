function OnLoad(executionContext)
{
    Xrm.WebApi.retrieveMultipleRecords("cp_customersetting", "?$select=cp_usetpa&$filter=cp_active eq true").then(
        function success(result) {
debugger;
            var formContext = executionContext.getFormContext(); 
            if (result.entities.length == 1 && result.entities[0].cp_usetpa)
            {
                formContext.getControl("cp_tpa").setVisible(true);
                }
            else
            {
                formContext.getControl("cp_tpa").setVisible(false);
            }
        },
        function (error) {
            console.log(error.message);
            // handle error conditions
        }
    );
}