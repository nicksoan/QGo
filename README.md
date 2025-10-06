# QGo

<div style="display: flex; align-items: left; gap: 20px;">
  <img width="100" alt="image" src="https://github.com/user-attachments/assets/011ceca5-686f-4f20-a23e-c9807b5d2ffa">
  <p>QGo is a versatile command parser application that allows users to execute various types of commands, including opening websites, accessing UNC paths, local folders, and running executable files. </p>
</div>

<img  alt="image" src="https://github.com/user-attachments/assets/b0b9b9f4-21f8-4dc8-9c26-9ac6841f6c83">


## Features

- **Open Websites**: Automatically open websites in the default browser.
- **Access UNC Paths**: Open network locations in File Explorer.
- **Open Local Folders**: Open local directories in File Explorer.
- **Run Executable Files**: Execute `.exe` files directly from the command parser.
- **Custom Shortcuts**: Define and use custom shortcuts for frequently used commands.

- **Parameterisation**: Each command can be parameterised.
  - Example to Google "Hello World":
    
![Image 1](https://github.com/user-attachments/assets/3a36110b-8074-4ea9-8eb5-cbe19d8fd78a)

![Image 2](https://github.com/user-attachments/assets/2541bf29-fb0b-46d3-99a9-a3348351e822)

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

<img width="370" alt="image" src="https://github.com/user-attachments/assets/f1940e78-5458-468d-af90-5631656d8886">

You can modify basic settings in QGo, such as 
- Default background colour
- Default font Colour
- Matched shortcut background colour
- Matched shortcut font colour
- Font size
- Wake shortcut

### Shortcuts
Right click on the input box and select "Edit Shortcuts" to open the list of Command/Shortcut pairs
Shortcuts are also defined in a JSON file which can be edited, but the program will require you to restart to register the changes. The shortcuts.json file is located in the Data folder where your QGo executable is stored.

#### Example `shortcuts.json`

### ToDo/Future Updates

#### WIP Improvements
- Update Taskbar icon
- 

#### Future Plans
- V2 restructuring to allow plugins
- Multipart shortcuts? (eg. Folders > Foldernames)
