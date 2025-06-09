import os
from PIL import Image

# --- Configuration ---
PIXEL_ART_SIZE = 100        # Desired size for the pixel art (e.g., 100x100)
MAX_LINES_PER_FILE = 2000   # Maximum number of lines per output .pw file

# --- User Input ---
# Remove extra quotes from user input to avoid path errors
script_dir=os.path.dirname(os.path.abspath("transformImg.py"))
image_path = input().strip('"')
output_folder_name = input().strip('"')

# Create the output folder if it doesn't exist
# os.path.dirname(image_path) gets the directory of the input image
base_output_directory = os.path.join(os.path.dirname(script_dir), "PixelWallEScripts")
os.makedirs(base_output_directory, exist_ok=True)


output_directory = os.path.join(base_output_directory, output_folder_name)
os.makedirs(output_directory, exist_ok=True)
print(f"Files will be saved to: {output_directory}")

try:
    image = Image.open(image_path).convert("RGBA")
except FileNotFoundError:
    print(f"Error: Image file not found at {image_path}. Please verify the path.")
    exit()

image = image.resize((PIXEL_ART_SIZE, PIXEL_ART_SIZE), Image.NEAREST)
width, height = image.size

all_generated_lines = []

# Add initial commands that always go at the beginning of the first .pw file
all_generated_lines.append("Spawn(0, 0)")
all_generated_lines.append("Size(1)")
all_generated_lines.append("y<-0")

for y_abs in range(height):
    for x_abs in range(width):
        r, g, b, a = image.getpixel((x_abs, y_abs))
        hex_color = f'"#{r:02x}{g:02x}{b:02x}{a:02x}"'
        
        all_generated_lines.append(f"Color({hex_color})")
        all_generated_lines.append("DrawLine(1, 0, 1)")

    if y_abs < height - 1:
        all_generated_lines.append(f"y<-{y_abs + 1}")
        all_generated_lines.append(f"ReSpawn(0, y)")

current_file_buffer = []
file_index = 0
last_pw_y = 0
generated_file_paths = [] # To store the paths of the generated files

# The first 3 lines (Spawn, Size, y<-0) always go in the first file.
# This ensures that the initial file has the base commands.
for i in range(min(3, len(all_generated_lines))):
    current_file_buffer.append(all_generated_lines[i])
    if all_generated_lines[i].startswith("y<-"):
        try:
            last_pw_y = int(all_generated_lines[i].split("<-")[1].strip())
        except ValueError:
            pass

# Iterate over the rest of the lines to divide them into files
for i in range(3, len(all_generated_lines)):
    line = all_generated_lines[i]

    # Update last_pw_y if the line is an assignment to 'y'
    if line.startswith("y<-"):
        try:
            last_pw_y = int(line.split("<-")[1].strip())
        except ValueError:
            pass

    # Determine if the line is an end-of-row command ('y' change or ReSpawn)
    is_end_of_row_command = line.startswith("y<-") or line.startswith("ReSpawn(0, y)")

    # If the buffer exceeds the line limit AND it's an end-of-row command,
    # and it's not the last line of the file (to avoid empty files),
    # then it's time to save the current file and start a new one.
    if len(current_file_buffer) + 1 > MAX_LINES_PER_FILE and is_end_of_row_command and i < len(all_generated_lines) - 1:
        output_file_name = f"walle_pixel_art_{file_index}.pw"
        output_path = os.path.join(output_directory, output_file_name)
        with open(output_path, "w") as f:
            f.write("\n".join(current_file_buffer))
        print(f"Generated file: {output_path} with {len(current_file_buffer)} lines.")
        generated_file_paths.append(output_path) # Save the path of the generated file

        current_file_buffer = [] # Clear the buffer for the new file
        file_index += 1 # Increment the file index
        
        # Add continuation commands to the beginning of the new file.
        # These commands position the drawing cursor at the correct 'y' to resume.
        current_file_buffer.append(f"Spawn(0, {last_pw_y})")
        current_file_buffer.append(f"y<-{last_pw_y}")
    
    # Add the current line to the buffer (either to the current file or the newly created one)
    current_file_buffer.append(line)

# After the loop finishes, write any remaining lines to the last file
if current_file_buffer:  # Make sure there's content to write
    output_file_name = f"walle_pixel_art_{file_index}.pw"
    output_path = os.path.join(output_directory, output_file_name)
    with open(output_path, "w") as f:
        f.write("\n".join(current_file_buffer))
    print(f"Generated file: {output_path} with {len(current_file_buffer)} lines.")
    generated_file_paths.append(output_path)  # Save the path of the last file
else:
    print("No lines were generated to write files. Possibly the image was empty or there was an error.")

print(f"\nPixel art generation process completed. Total files generated: {len(generated_file_paths)}.")

# --- Create the master .pw file ---
master_file_content = ["Spawn(0,0)"]  # Always starts with Spawn(0,0) in the master
for path in generated_file_paths:
    # Add a Run command for each generated .pw file.
    # Make sure the paths are correct for your PW interpreter to find them.
    # Double quotes are used around the path for compatibility.
    master_file_content.append(f"Run(\"{path}\")")

master_file_name = f"{output_folder_name}.pw"
master_file_path = os.path.join(output_directory, master_file_name)

with open(master_file_path, "w") as f:
    f.write("\n".join(master_file_content))
print(f"\nMaster file generated: {master_file_path}")