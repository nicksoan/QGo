# QGo

<div style="display: flex; align-items: left; gap: 20px;">
  <img width="100" height="100" alt="QGoLogo" src="https://github.com/user-attachments/assets/e6de29c0-4c76-4c19-aff6-a203d0188549" />
  <p>QGo is a versatile command parser application that allows users to execute various types of commands, including opening websites, accessing UNC paths, local folders, and running executable files. </p>
</div>




![QGoDemo](https://github.com/user-attachments/assets/22c61213-16f0-40ba-88ac-d68cebe060f3)



## Features

- **Open Websites**: Automatically open websites in the default browser.
- **Access UNC Paths**: Open network locations in File Explorer.
- **Open Local Folders**: Open local directories in File Explorer.
- **Run Executable Files**: Execute `.exe` files directly from the command parser.
- **Custom Shortcuts**: Define and use custom shortcuts for frequently used commands.

- **Parameterisation**: Each command can be parameterised.
  - Example to Google "Hello World":
    
<img width="227" height="54" alt="image" src="https://github.com/user-attachments/assets/66faef59-57e6-4fab-bb43-4d56a428423e" />


<img width="821" height="321" alt="image" src="https://github.com/user-attachments/assets/05000c6d-49ee-413d-80d0-12ba0ff11b51" />


## Usage


### Command Matching

- **Websites**: Commands containing a URL starting with `http://` or `https://` will be opened in the default browser.
- **UNC Paths**: UNC paths starting with `\\` will be opened in File Explorer.
- **Local Folders**: Local folder paths (e.g., `C:\Users\MyUserId\Documents`) will be opened in File Explorer.
- **Executable Files**: Exe files (e.g., `C:\Program Files\MyApp\app.exe`) will be run.

### Example Commands
```
{
  "google": "https://www.google.com/search?q={param}",
  "documents": "%HOMEPATH%",
  "networkshare": "\\\\server\\sharedfolder",
  "youtube": "https://www.youtube.com/results?search_query={param}",
  "reddit": "https://reddit.com",
  "github": "https://github.com/",
  "temp": "C:\temp",
  "qgo": "https://github.com/nicksoan/QGo",
  "repo": "%USERPROFILE%",
  "notepad": "C:\\Program Files (x86)\\Notepad++\\notepad++.exe"

}
```

## Configuration

<img width="302" height="177" alt="image" src="https://github.com/user-attachments/assets/1d31b501-740f-4c36-851a-6c55a36797ef" />

You can modify basic settings in QGo, such as 
- Default background colour
- Default font Colour
- Font size
- Wake shortcut

### Shortcuts
<img width="406" height="242" alt="image" src="https://github.com/user-attachments/assets/44fc3977-fc78-4e46-9b12-7de1fd57e290" />

Right click on the input box and select "Edit Shortcuts" to open the list of Command/Shortcut pairs
Shortcuts are also defined in a JSON file which can be edited, but the program will require you to restart to register the changes. The shortcuts.json file is located in the Data folder where your QGo executable is stored.

#### Example `shortcuts.json`

### ToDo/Future Updates

#### WIP Improvements
- Update Taskbar icon
- Resizing/Screen Positioning
- Autocomplete highlighting / tab for params
- Trigger multiple actions
- Toggle System settings (Display, sound, etc...)
- Import/Export Settings & Shortcuts

#### Future Plans
- V2 restructuring to allow plugins
- Multipart shortcuts? (eg. Folders > Foldernames)
