# PerspectiveCorrectionTool

This project is a .NET C# based image processing application that uses the AForge library. During my internship, I developed this tool to capture the video stream from a camera and perform real-time perspective correction based on four selected points. The corrected image is then displayed on the screen.

## Features

- **Real-time Video Stream**: Captures live video feed from a connected camera.
- **Perspective Correction**: Automatically corrects the perspective of the captured video based on four user-selected points.
- **User-Friendly Interface**: Allows users to easily select points for perspective correction.

## Installation

1. **Prerequisites**:
    - .NET Framework installed on your machine.
    - AForge.NET library.

2. **Clone the Repository**:
    ```bash
    git clone https://github.com/yourusername/PerspectiveCorrectionTool.git
    ```

3. **Open the Project**:
    - Open the project in Visual Studio.

4. **Restore NuGet Packages**:
    - Restore the required NuGet packages for the AForge library.

5. **Build the Project**:
    - Build the project to ensure all dependencies are correctly installed.

## Usage

1. **Connect a Camera**:
    - Ensure your camera is connected and recognized by your system.

2. **Run the Application**:
    - Start the application from Visual Studio or the executable file.

3. **Select Points for Perspective Correction**:
    - Use the interface to select four points on the video feed that define the quadrilateral area you want to correct.

4. **View Corrected Video**:
    - The application will automatically correct the perspective and display the corrected video in real-time.

## Benefits of Automation

- **Improved Accuracy**: Automating perspective correction ensures consistent and precise adjustments, reducing human error.
- **Increased Efficiency**: Real-time correction allows for immediate results, saving time in manual adjustments.
- **Enhanced Usability**: Simplifies the process of perspective correction, making it accessible to users without specialized knowledge.
- **Scalability**: Can be easily integrated into larger automation systems for various applications such as surveillance, quality control, and virtual reality.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Contact

For any inquiries or further information, please contact me at [your email address].

---

Feel free to update this README.md file with additional information as needed.
