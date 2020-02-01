| [docs](..)  / [using](.) / base-policy.md
|:---|

# Creating a new Base WDAC Policy

1. *Select the New Policy Editor from the start menu*

![](imgs/new-hover.png)

1. *Select the Base Policy option page*

If the default setting (see [Settings Page](using/settings-page.md)) is enabled, 
the base policy option will be pre-selected. Otherwise, select the base policy option. 

![](imgs/new-base.png)

1. *Select from one of the default templates*

Each one of the templates has 

  * *Allow Microsoft Mode*: 
  
  * *Windows Works Mode*:
  
  * *Signed and Reputable Mode*:

![](imgs/new-base-template.png)

At any time during the workflow, you can choose to return to the default template page by selecting the `Policy Template` button on the left-hand menu. *Note:* returning back to the template page will remove the configured policy rule options 
as well as the custom signing rules.  
  
1. *Configure the policy rule options*

Upon page launch, policy rule options will be automatically enabled/disabled depending on the chosen template from the previous page. Choose to enable or disable the desired policy rule options by pressing the slider button next to 
the policy rule titles. 

Hovering the mouse over the policy rule names will display a short description of the rule
at the bottom of the page. More information about each of the policy rules can be located at the [WDAC policy rules](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/select-types-of-rules-to-create#windows-defender-application-control-policy-rules)
page. 

Selecting the `+ Advanced Options` button will reveal the advanced policy rule options panel. This grouping of rules contains additional policy rule options which are less common to a majority of users. 

Lastly, *Audit Mode* is enabled by default for all of the templates. We recommend leaving the Audit Mode policy rule option enabled until users have sufficiently understood how the policy and signing rules will affect their case. 
Disabling Audit Mode will result in the policy running in enforced mode after the policy is deployed. For more information on deploying WDAC policies see [Deploying WDAC Policies](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/windows-defender-application-control-deployment-guide). 

1. *Creating policy signing rules*

The Signing Rules List on the left-hand side of the page document the pre-set signing rules of the template as well as the exceptions. 


*Creating Custom Signing Rules*

Selecting the `+ Custom Rules` button will open the Custom Rules panel. Four types of custom rules conditions can be defined. 

| Rule Condition | Usage Scenario | 
| - | - |
| Publisher | To use a publisher condition, the files must be digitally signed by the software publisher, or you must do so by using an internal certificate. Rules that are specified to the version level might have to be updated when a new version of the file is released.|
| File Path | Any file can be assigned this rule condition; however, because path rules specify locations within the file system, any subdirectory will also be affected by the rule (unless explicitly exempted).|
| Folder Path | Any folder can be assigned this rule condition; .|
| File Hash | Any file can be assigned this rule condition; however, the rule must be updated each time a new version of the file is released because the hash value is based in part upon the version.|

  1. *Publisher Rules* - select the Publisher option from the Rule Type combobox. Next choose to Allow or Deny the publisher, and select a reference file signed by the software publisher off which to base the rule. 
  By default, the publisher is set to apply to all files signed by the publisher, with the specific product name and file name with a version at or above the one specified. The restrictiveness of the rule can be modified using the slider. 
  The text below the slider documents outlines the how the rule will be interpreted. 
  
  The table below shows the relationship between the slider placement, the corresponding WDAC rule level and its description. 
  
  | Rule Condition | WDAC Rule Level | Description |
  | - | - | - |
  | *Publisher* | FilePublisher | Combination of the file name, PCA cert with CN of the leaf, and the minimum version number. |
  | *Product name* | FilePublisher | Combination of the file name, PCA cert with CN of the leaf, and the minimum version number. |
  | *File name* | FilePublisher | Combination of the file name, PCA cert with CN of the leaf, and the minimum version number. |
  | *Version* | FilePublisher | Combination of the file name, PCA cert with CN of the leaf, and the minimum version number. |
  
  _this section needs to be revised_
  
  1. *Path Rules* - select the Path option from the Rule Type combobox. Next choose to Allow or Deny the path, and select either a File or Folder rule using the radiobutton below the Browse button. Lastly, select the reference file
  or folder off which to base the rule. 
  
  1. *Hash Rules* - select the File Hash option from the Rule Type combobox. Next choose to Allow or Deny the hash, and select the file off which to base the rule. 
  
*Deleting Signing Rules*
  
Template signing rules and custom rules can be deleted from the policy by selecting the rule from the rules list dataviewer. Once the rule is highlighted, selecting the delete button underneath the table will prompt for additional confirmation. 
Selecting `Yes` will remove the rule from the policy and the rules table. 


1. *Building the policy*

The policy build page will monitor the progress of the WDAC policy creation process. Depending on the number and complexity of the custom signing rules, the build process could take several minutes. 
Once the build process is complete, selecting the policy path link will open the policy XML file for review. The binary file is also written to the same path for manual deployment. Steps for manual deployment
can be reviewed here [Deploying WDAC Policies](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/windows-defender-application-control-deployment-guide). 
