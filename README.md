# Perspective Correction Tool

A simple .NET Windows Forms application for real-time perspective correction of a video stream. This tool, originally developed during an internship, captures video from a camera, allows the user to select four points defining a quadrilateral, and then transforms the perspective of the video feed to that rectangle.

This repository contains a refactored and modernized version of the original project, making it easier to understand, maintain, and contribute to.

## Features

- **Real-time Video Stream**: Captures a live video feed from any connected camera recognized by Windows.
- **Interactive Perspective Correction**: Click four points on the video to define the area to be corrected.
- **Toggle View**: Switch between the original and the perspective-corrected view.

## Screenshots

![Application Screenshot](screenshot.png)

## Installation

1.  **Prerequisites**:
    *   Windows Operating System.
    *   [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) or later.

2.  **Clone the Repository**:
    ```bash
    git clone https://github.com/yourusername/PerspectiveCorrectionTool.git
    ```

3.  **Open and Build**:
    *   Open `PerspectiveCorrectionTool.sln` in Visual Studio.
    *   Build the solution (press `Ctrl+Shift+B`). NuGet packages should be restored automatically.

## Usage

1.  **Connect a Camera**:
    - Ensure your camera is connected and recognized by your system.

2.  **Run the Application**:
    - Start the application from Visual Studio (press `F5`) or run the executable from the `bin/Debug` or `bin/Release` folder.

3.  **Select a Camera**:
    - Choose your camera from the dropdown menu and click "Start".

4.  **Select Points for Correction**:
    - Click four points on the video feed to define the quadrilateral area you want to correct.
    - After selecting four points, click the "Apply Perspective" button.

5.  **View Corrected Video**:
    - The application will display the corrected video in real-time.
    - You can toggle between the original and corrected view using the same button.
    - To select new points, click on the image again to clear the selection.

## Contributing

Contributions are welcome! If you have suggestions for improvements or want to fix a bug, please feel free to:

1.  **Fork** the repository.
2.  Create a new **branch** for your feature or fix.
3.  Make your changes.
4.  Submit a **pull request**.

Please try to follow the existing code style and add comments for any new logic.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
