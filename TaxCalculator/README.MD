# Read
Read Codding rules.docx before start coding.

# Run .net with:
** Same location as the README **
```
dotnet watch
```

# Undo local commit but keep the changes
```
git reset --soft HEAD~1
```


Config/
+-- default/
¦   +-- deduction/
¦   ¦   +-- DonationWorkflow.json
¦   ¦   +-- PensionWorkflow.json
¦   +-- tax/
¦   ¦   +-- IncomeTaxWorkflow.json
¦   ¦   +-- CapitalGainsWorkflow.json
¦   +-- credit/
¦       +-- WomenCreditWorkflow.json
¦       +-- ChildrenCreditWorkflow.json
+-- 2023/
¦   +-- deduction/
¦   ¦   +-- EducationFundWorkflow.json
¦   +-- tax/
¦   ¦   +-- ForeignIncomeTaxWorkflow.json
¦   +-- credit/
¦       +-- SelfEmploymentCreditWorkflow.json