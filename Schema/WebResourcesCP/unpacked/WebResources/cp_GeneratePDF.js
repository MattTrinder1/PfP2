function GeneratePDFDocument(formContext,settingName)
{
	//alert('generate pdf');
    var entityId   = formContext.data.entity.getId();
	debugger;
    entityId = entityId.replace(/[{}]/g, "")
    var pnId = {recordId:entityId};
    
    debugger;
    var environments = getEnvironment(Xrm.Utility.getGlobalContext().getClientUrl());
    var urlSetting = getUrlSetting(settingName +" (" + environments[0].cp_name + ")",environments[0].cp_environmentid);

	var dataJson = window.JSON.stringify(pnId);
	try
	{
    	var req = new XMLHttpRequest();
        req.open("POST", urlSetting[0].cp_value, false); //Action will be invoked Asynchronously
		req.setRequestHeader("Accept", "application/json");
		req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
		req.setRequestHeader("OData-MaxVersion", "4.0");
		req.setRequestHeader("OData-Version", "4.0");
		req.onreadystatechange = function ()
		{debugger;
			if (this.readyState === 4)
			{
				req.onreadystatechange = null;
				if (this.status === 204 || this.status === 200 || this.status === 202)
				{
					if (this.statusText === "No Content" || this.statusText === "") // In case of 204
					var response = req.response;
					else
					{
						var response = JSON.parse(req.response);
					}
                    Xrm.Utility.alertDialog("Document generated");

    			}
				else
				{
					var error = JSON.parse(req.response).error;
    				Xrm.Utility.alertDialog(error.message);
				}
			}
		};
		req.send(dataJson);
	}
	catch (e)
	{
    	Xrm.Utility.alertDialog(e.message);
	}
}


function getEnvironment(url)
{
	var req2 = new XMLHttpRequest();
	req2.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + "cp_environments?$filter=cp_orgurlname eq '" + url + "'"  , false);
	req2.setRequestHeader("OData-MaxVersion", "4.0");
	req2.setRequestHeader("OData-Version", "4.0");
	req2.setRequestHeader("Accept", "application/json");
	req2.setRequestHeader("Content-Type", "application/json; charset=utf-8");
	req2.send();
	return JSON.parse(req2.response).value;
}

function getUrlSetting(settingName, environmentid)
{
	var req2 = new XMLHttpRequest();
	req2.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + "cp_systemsettings?$filter=cp_name eq '" + settingName + "' and _cp_environment_value eq " + environmentid  , false);
	req2.setRequestHeader("OData-MaxVersion", "4.0");
	req2.setRequestHeader("OData-Version", "4.0");
	req2.setRequestHeader("Accept", "application/json");
	req2.setRequestHeader("Content-Type", "application/json; charset=utf-8");
	req2.send();
	return JSON.parse(req2.response).value;
}