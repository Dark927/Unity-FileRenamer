### **Short Description (English)**  
The **File Renamer** tool for Unity Editor allows users to batch rename and organize image files with customizable naming templates. It supports multiple options.

---

### **File Renamer - Unity Editor Tool**  
#### **Overview**  
The **File Renamer** is a Unity Editor extension designed to streamline the process of renaming and organizing image files. It provides an intuitive UI for selecting, sorting, and renaming multiple files with customizable templates.

#### **Features**  
✅ **Batch File Selection** – Choose multiple image files at once.  
✅ **Custom Naming Templates** – Define a base name for the new files.  
✅ **Automatic Numbering** – Append sequential numbers to filenames.  
✅ **Preserve Existing Numbering** – Keep existing numbers from filenames.  
✅ **Sorting Options** – Sort files in ascending or descending order.  
✅ **Overwrite Existing Files** – Choose whether to replace existing files.  
✅ **Subfolder Creation** – Organize renamed files into a subfolder.  
✅ **Error Handling** – Prevents overwriting issues and alerts for missing files.  

---

### **How to Use**  
#### **1. Open the File Renamer Window**  
- Go to `Tools > File Renamer` in the Unity Editor.

#### **2. Select Image Files**  
- Click the `Select Files` button.  
- Choose multiple image files (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.tiff`).  
- The tool will display the number of selected files.

#### **3. Define the File Naming Template**  
- Enter a base filename in the `File Name Template` field.  
- (Optional) Click `Select template from file` to extract a name from an existing file.

#### **4. Configure Renaming Options**  
- **Can Overwrite Files** – Allow overwriting existing files.  
- **Sort Ascending** – Sort files alphabetically before renaming.  
- **Add Numbering** – Append sequential numbers to filenames ("_01", "_02", etc.).  
- **Preserve Existing Numbering** – Keep numbers from original filenames.  
- **Create Subfolder** – Save renamed files in a new subfolder.

#### **5. Process Files**  
- Click `Process Files`.  
- Select an export folder.  
- The files will be renamed and copied to the chosen location.  

---

### **File Naming Logic**  
- If **preserve existing numbering** is enabled, filenames will retain numbers found at the end.  
- If **add numbering** is enabled, files will be numbered sequentially (`image_01.png`, `image_02.png`, etc.).  
- The tool maintains the original file extensions.

---

### **Example**  
#### **Before Renaming:**  
📂 `photo1.png`, `photo2.png`, `snapshot3.jpg`  

#### **After Renaming (Template: "image", Add Numbering ON):**  
📂 `image_01.png`, `image_02.png`, `image_03.jpg`  

#### **After Renaming (Template: "image", Preserve Numbering ON):**  
📂 `image_1.png`, `image_2.png`, `image_3.jpg`  

---

### **Debugging & Logs**  
- Errors (e.g., no files selected, missing export folder) are logged in Unity’s Console.  
- If no valid files exist, the tool prompts a warning and resets selection.  

---

### **Future Improvements**  
🔹 Support for additional file types.  
🔹 More advanced sorting and filtering options.  
🔹 Customizable numbering formats (e.g., `image-001.png`).  
