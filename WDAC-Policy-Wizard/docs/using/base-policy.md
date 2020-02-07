| [docs](..)  / [using](.) / base-policy.md
|:---|

# Creating a new Base WDAC Policy

This document outlines the steps to create a new WDAC code integrity (CI) policy while using one of the three available CI templates
as a starting point. The tool enables users to configure the policy rules, its signing rules and its attributes. 

## 1. Select the Policy Creator from the start menu, then Base Policy ##

If the default setting (see [Settings Page](using/settings-page.md)) is enabled, the base policy option will be pre-selected. Otherwise, select the base policy option. 

[![Little red ridning hood](../imgs/new-hover.png)](../imgs/wdac-new.mp4 "Video title")

## 2. Select from one of the default template policies ##

Each one of the template policies has a unique set of policy rules and a varying level of security. 

| Template Policy | Authorizes the  | Circle of Trust |
| - | - | - |
|**Allow Microsoft Mode** | Microsoft Office365 Applications <br/> Microsoft Store Applications | Smallest Circle-of-Trust |
|**Windows Works Mode** | Microsoft Office365 Applications <br/> Windows-signed Applications <br/> WHQL Kernel Drivers |  |
|**Signed and Reputable Mode** | Microsoft Office365 Applications <br/> Microsoft Store Applications <br/> Windows-signed Applications <br/> WHQL Kernel Drivers <br/> [Files with good reputation, according to the ISG](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/use-windows-defender-application-control-with-intelligent-security-graph) | Largest Circle-of-Trust |

The policy name and file location will default based on the template policy selected. The policy name and file location can be set be selecting the textbox and typing the desired string. 

At any time during the workflow, you can choose to return to the default template page by selecting the `Policy Template` button on the left-hand menu. 

**NOTE:** Returning to the template page will remove the configured policy rule options as well as the custom signing rules.  

![](imgs/new-base-template.png)


## 3. Configure the policy rule options ##

Upon page launch, policy rule options will be automatically enabled/disabled depending on the chosen template from the previous page. Choose to enable or disable the desired policy rule options by pressing the slider button next to 
the policy rule titles. 

Hovering the mouse over the policy rule names will display a short description of the rule
at the bottom of the page. More information about each of the policy rules can be located at the [WDAC policy rules](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/select-types-of-rules-to-create#windows-defender-application-control-policy-rules)
page. 

Selecting the `+ Advanced Options` button will reveal the advanced policy rule options panel. This grouping of rules contains additional policy rule options which are less common to a majority of users. 

Lastly, **Audit Mode** is enabled by default for all of the templates. We recommend leaving the Audit Mode policy rule option enabled until users have sufficiently understood how the policy and signing rules will affect their scenario. 
Disabling Audit Mode will result in the policy running in enforced mode after the policy is deployed. For more information on deploying WDAC policies see [Deploying WDAC Policies](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/windows-defender-application-control-deployment-guide). 

## 4. Creating policy signing rules ## 

The Signing Rules List on the left-hand side of the page document the pre-set signing rules of the template as well as the exceptions. 

#### Creating Custom Signing Rules ####

Selecting the `+ Custom Rules` button will open the Custom Rules panel. Four types of custom rules conditions can be defined. 

| Rule Condition | Usage Scenario | 
| - | - |
| Publisher | To use a publisher condition, the files must be digitally signed by the software publisher, or you must sign with an internal certificate. |
| File Path | Any file can be assigned this rule condition; however, because path rules specify locations within the file system, any subdirectory will also be affected by the rule (unless explicitly exempted).|
| Folder Path | Any folder and subfolder can be assigned this rule condition (unless explicitly exempted).|
| File Hash | Any file can be assigned this rule condition; however, the rule must be updated each time a new version of the file is released because the hash value is based in part upon the version.|

  1. **Publisher Rules** - select the Publisher option from the Rule Type combobox. Next choose to Allow or Deny the publisher, and select a reference file signed by the software publisher off which to base the rule. 
  By default, the publisher is set to apply to all files signed by the publisher, with the specific product name and file name with a version at or above the one specified. The restrictiveness of the rule can be modified using the slider. 
  The text below the slider documents outlines the how the rule will be interpreted. 
  
  The table below shows the relationship between the slider placement, the corresponding WDAC rule level and its description. The lower the placement on the table and the UI slider, the greater the specificity of the rule. 
  
  | Rule Condition | WDAC Rule Level | Description |
  | - | - | - |
  | **Publisher** | PCACertificate | Highest available certificate is added to the signers. This is typically the PCA certificate, one level below the root certificate. Any file signed by this certificate will be affected. |
  | **Product name** | Publisher | This rule is a combination of the PCACertificate rule and the common name (CN) of the leaf certificate. Any file signed by a major CA but with a leaf from a specific company, for example a device driver corp, is affected. |
  | **File name** | SignedVersion | This rule is a combination of PCACertificate, Publisher and a version number. Anything from the specified publisher with a version at or above the one specified is affected. |
  | **Version** | FilePublisher | Most specific. Combination of the file name, publisher and PCA certificate as well as a minimum version number. Files from the publisher with the specified name and greater or equal to the specified version are affected. |
    
  2. **Path Rules** - select the Path option from the Rule Type combobox. Next choose to Allow or Deny the path, and select either a File or Folder rule using the radiobutton below the Browse button. Lastly, select the reference file
  or folder off which to base the rule. 
  
  3. **Hash Rules** - select the File Hash option from the Rule Type combobox. Next choose to Allow or Deny the hash, and select the file off which to base the rule. 
  
#### Deleting Signing Rules ####
  
Template signing rules and custom rules can be deleted from the policy by selecting the rule from the rules list dataviewer. Once the rule is highlighted, selecting the delete button underneath the table will prompt for additional confirmation. Select `Yes` to remove the rule from the policy and the rules table. 


## 5. Building the policy ##

The policy build page will monitor the progress of the WDAC policy creation process. Depending on the number and complexity of the custom signing rules, the build process could take several minutes. 

Once the build process is complete, selecting the policy path link will open the policy XML file for review. The binary file is also written to the same path for manual deployment. Steps for manual deployment can be reviewed here [Deploying WDAC Policies](https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/windows-defender-application-control-deployment-guide). 
