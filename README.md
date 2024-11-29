# QGo

QGo is a versatile command parser application that allows users to execute various types of commands, including opening websites, accessing UNC paths, opening local folders, and running executable files. The application also supports custom shortcuts for frequently used commands.

## Features

- **Open Websites**: Automatically open websites in the default browser.
- **Access UNC Paths**: Open network locations in File Explorer.
- **Open Local Folders**: Open local directories in File Explorer.
- **Run Executable Files**: Execute `.exe` files directly from the command parser.
- **Custom Shortcuts**: Define and use custom shortcuts for frequently used commands.

## Usage

### Commands

- **Websites**: Enter a URL starting with `http://` or `https://` to open it in the default browser.
- **UNC Paths**: Enter a UNC path starting with `\\` to open it in File Explorer.
- **Local Folders**: Enter a local folder path (e.g., `C:\Users\MyUserId\Documents`) to open it in File Explorer.
- **Executable Files**: Enter the path to an executable file (e.g., `C:\Program Files\MyApp\app.exe`) to run it.
- **Shortcuts**: Use predefined shortcuts to quickly execute commands. Shortcuts can include placeholders for parameters.

### Example Commands

- Open a website: `https://www.example.com`
- Access a UNC path: `\\Server\Share`
- Open a local folder: `C:\Users\MyUserId\Documents`
- Run an executable file: `C:\Program Files\MyApp\app.exe`
- Use a shortcut: `myshortcut {param}`

## Configuration

### Shortcuts

Shortcuts are defined in a JSON file. The JSON file should be located at a specified path and contain key-value pairs where the key is the shortcut name and the value is the command.

#### Example `shortcuts.json`
