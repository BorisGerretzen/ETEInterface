# ETEInterface
[![Build Status](https://dev.azure.com/bortgerres/ETEInterface/_apis/build/status/BorisGerretzen.ETEInterface?branchName=master)](https://dev.azure.com/bortgerres/ETEInterface/_build/latest?definitionId=4&branchName=master)
[![Build Status](https://dev.azure.com/bortgerres/ETEInterface/_apis/build/status/BorisGerretzen.ETEInterface?branchName=staging)](https://dev.azure.com/bortgerres/ETEInterface/_build/latest?definitionId=4&branchName=staging)

When my girlfriend was working on her BSc. thesis at the chair of Elastomer Technology and Engineering at the University of Twente, she had to do a lot of manual data processing. 
I made a pretty limited python script that could do this and generate graphs, but it wasn't very intuitive to use and a bit buggy overall, so I made a new version in C#.


## Functionality
This program converts a directory filled with .xls files from the tensile machine, to a single .xlsx file with all of the aggregated results.
In addition to this, it is possible to import one of these aggregated .xlsx files and generate graphs from the data automatically. 

### Aggregation
- .xls file support
- *.zs2 support is coming*
- Recursive file search
- Supported data:
  - Tensile
  - Tear
  - *more to come*
- Supported columns:
  - min
  - mean
  - max
  - error bottom
  - error top

### Graphing
- Graphing 2 series in one graph
- Selecting series combinations through UI
- Saving graph templates so they can easily be generated later with different options
- Custom color settings for each series
- Custom X and Y axis headers
- *legend is coming*
- *graph title is coming*

## How to use
### Aggregating files
- Select columns using the checkboxes on the top left
![select columns](https://user-images.githubusercontent.com/15902678/157565384-4d14f40b-e225-496b-ae9e-db476fc005e2.png)
- Choose if you want to add a sheet to a file or create a new file, **if you choose create a new file and select an existing file, it will be overwritten**\
![select file](https://user-images.githubusercontent.com/15902678/157565392-0acd7936-46d2-4321-a0a8-e46d85338685.png)
- Pick the directory of the input files and select the output file
![select inputoutput](https://user-images.githubusercontent.com/15902678/157565398-17e57ae8-7427-46ad-a183-e60cb68da7f2.png)
- Check this checkbox if your files are in subdirectories of the directory you picked, eg. you picked directory documents/data/, but your files are in documents/data/1, documents/data/2, etc.\
![recurse](https://user-images.githubusercontent.com/15902678/157565513-bf01261f-542a-4e8e-befb-cc1e4f8c4b73.png)
- Press the export button and wait for the popup message!

## Creating graphs
The process of creating graphs is a little more complicated to set up, but once it is set up and a template has been created, the process is very fast.

### Creating a template
- Open your aggregated .xlsx file and make sure there is a column for each category you wish to filter on. For example, if you're researching the properties of different combinations of plastics and rubbers, and their respective ratios, you want the plastic-rubber combination to be a column, and the ratio to be a column.
![EXCEL_OIcmkkGkKm](https://user-images.githubusercontent.com/15902678/157566584-70e4e43f-ab4e-467f-b6bc-e10a4b5bf94e.png)
- Open the file in the graphs tab of the program
![select data](https://user-images.githubusercontent.com/15902678/157566664-a3b68ac1-1504-4152-b643-756044982467.png)
- Select the sheet you want to make graphs from
![Interface_P3SIsCOlRT](https://user-images.githubusercontent.com/15902678/157566781-26b960db-8431-43af-98e3-44e92f01a11c.png)
- After you select the sheet, the data should appear in the data tab on the right, hold your left mousebutton while you drag your mouse over the columns you want to select as categories, in this case compound and ratio. After you did this, press the 'Select categories' button.
![Interface_I5OjFMRWzg](https://user-images.githubusercontent.com/15902678/157566966-718a7140-0056-4dfd-a97c-2130db87763e.png)
- Press the 'View combinations' button to add different combinations to graph. You have to select '\*' for the column you want to have on the x axis, **You have to select '\*' for one column and only one column in the table**, in this case, that column is ratio.\
![Interface_8ppEgS47VN](https://user-images.githubusercontent.com/15902678/157567421-f0f9e730-fc44-40c7-9813-0b94f9604af6.png)
- Close the combinations window 
- Open the layout tab\
![Interface_ePGQIODZ47](https://user-images.githubusercontent.com/15902678/157567570-1dad1817-49a9-4cd4-b9c6-298480ce735a.png)
- Use an [online color picker](https://htmlcolorcodes.com/color-picker/) to choose colors for the first and second series
- Enter the hex colors in the color 1 and color 2 textboxes, **do not forget the #**
- Enter text for the X and Y axis in their respective textboxes
- Open the graph tab to see a render of the first graph in your selected combinations
![Interface_hsMYZkkyan](https://user-images.githubusercontent.com/15902678/157567938-4ff77f90-7576-4300-aab3-a485b6e979cb.png)
- If you are satisfied with the results, enter a name for your template in the textbox on the left and press the 'Save as template' button
![Interface_1xVorVjnGw](https://user-images.githubusercontent.com/15902678/157568027-5cdb072d-2e73-4425-af2e-f5c87899b7dd.png)
- If you want to export your graphs, press the export button in the bottom right. A window will appear where you can select a directory for the graphs.

### Loading a template
Loading a template is a lot easier than creating one, follow the steps below for a quick guide.
- Open the .xlsx file you created the template for in the graphs tab of the program
![select data](https://user-images.githubusercontent.com/15902678/157566664-a3b68ac1-1504-4152-b643-756044982467.png)
- Press the 'Load from template' button in the bottom right
![Interface_8NhZjcjsrC](https://user-images.githubusercontent.com/15902678/157568634-4ca20f73-137b-436e-bb42-0046d78d1b9c.png)
- You should get a popup saying the import is succesful, you can now change some settings and press export to export all the graphs
- If you made changes you want to keep for later, make sure to press the save as template button again, the template name should already be filled in.\
![Interface_1xVorVjnGw](https://user-images.githubusercontent.com/15902678/157568027-5cdb072d-2e73-4425-af2e-f5c87899b7dd.png)
