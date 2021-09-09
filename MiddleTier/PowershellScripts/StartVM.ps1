
#Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force 
#connect-azaccount 

#set the environment name here
start-azvm -Name opnet-dev-sql -ResourceGroupName MOD-OPNet -NoWait 
start-azvm -Name maritimeew -ResourceGroupName MOD-OPNet -NoWait
start-azvm -Name powerappdev1 -ResourceGroupName CumbriaDevVMS -NoWait
start-azvm -Name modtesthost -ResourceGroupName MOD -NoWait