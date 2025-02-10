
# **File Renamer - Unity Editor Tool**

![previewImage](https://github.com/user-attachments/assets/5e2524fe-0d64-4ba7-bc7d-fa2a0dc2d617)


## **Overview**
**File Renamer** is a **Unity Editor extension** that streamlines batch renaming and organization of image files. It provides an intuitive **UI for selecting, previewing, and renaming multiple files** with customizable templates.

The tool offers various **sorting, numbering, and folder organization options**, ensuring flexibility and control over file naming conventions.

---

## **Features**
✅ **Batch File Selection** – Select multiple image files at once.  
✅ **Custom Naming Templates** – Define a base name for the new files.  
✅ **Live Filename Preview** – See the new filenames before applying changes.  
✅ **Automatic Numbering** – Append sequential numbers to filenames (`_01`, `_02`, etc.).  
✅ **Preserve Existing Numbering** – Retains numbers found in original filenames.  
✅ **Sorting Options** – Sort files in ascending or descending order.  
✅ **Remove Files from Selection** – Remove specific files before processing.  
✅ **Overwrite Existing Files** – Choose whether to replace existing files.  
✅ **Subfolder Creation** – Save renamed files into a subfolder.  
✅ **Open Export Folder** – Automatically open the destination folder after renaming.  
✅ **Error Handling & Logs** – Displays warnings for missing files or naming conflicts.  

---

## **How to Use**

### **1. Open the File Renamer Window**
- Navigate to `Tools > File Renamer` in the Unity Editor.

### **2. Select Image Files**
- Click **"Select Files"** to open the file selection dialog.
- Choose multiple image files (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.tiff`).
- The tool will display the **number of selected files**.

### **3. Manage Selected Files**
- Toggle **"Show File List"** to preview selected files.  
- **Remove individual files** from the selection using the `[rm]` button.  

### **4. Define the File Naming Template**
- Enter a base name in the **"File Name Template"** field.  
- (Optional) Click **"Select template from file"** to extract a name from an existing file.  

### **5. Configure Renaming Options**
- **Can Overwrite Files** – Allow overwriting existing files.  
- **Sort Ascending** – Sort files alphabetically before renaming.  
- **Add Numbering** – Append sequential numbers to filenames (`_01`, `_02`, etc.).  
- **Preserve Existing Numbering** – Retain numbers from original filenames.  
- **Create Subfolder** – Save renamed files into a new subfolder.  
- **Open Export Folder** – Automatically open the output folder after renaming.  

### **6. Preview the Result**
- Toggle **"Preview Result"** to see a **live preview** of the renaming process.  
- The tool displays filenames in the format:  
  ```
  oldFileName.png → newFileName_01.png
  ```

### **7. Process & Export Files**
- Click **"Process Files"** to apply changes.
- The tool will generate new filenames and export the files.
- If **"Open Export Folder"** is enabled, the destination folder will open automatically.

---

## **File Naming Logic**
- If **Preserve Existing Numbering** is enabled, filenames **retain** numbers found in the original name.
- If **Add Numbering** is enabled, files are **sequentially numbered** (`image_01.png`, `image_02.png`, etc.).
- The tool **maintains the original file extensions**.

---

## **Example Usage**

### **Before Renaming:**
📂 `photo1.png`, `photo2.png`, `snapshot3.jpg`  

### **After Renaming (Template: "image", Add Numbering ON):**
📂 `image_01.png`, `image_02.png`, `image_03.jpg`  

### **After Renaming (Template: "image", Preserve Numbering ON):**
📂 `image_1.png`, `image_2.png`, `image_3.jpg`  

---

## **Error Handling & Debugging**
- **No files selected?** → A warning will appear in Unity’s Console.  
- **Missing export folder?** → The tool prompts for a valid destination.  
- **File conflicts?** → The tool prevents accidental overwriting if not allowed.  

---

### **📌 Notes**
This tool is designed for **Unity Editor use only** and does not work in a standalone build.  

**File Renamer** helps speed up asset organization for game development, UI design, and more!
