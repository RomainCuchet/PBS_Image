using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppVisuals;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;

namespace PBS_Image
{
    internal class Interface
    {
        static string racine = "../../../";
        static List<string> filters = new() { "sharpness", "box blur", "edge1", "edge2", "edge3" };

        

        public static string choose_file(TableSelector file_selector,string folder = "Images/Save")
        {
            List<string> files = Directory.GetFiles(racine+folder, "*.bmp").Select(Path.GetFileName).ToList();
            file_selector.ClearLines();
            foreach(string file in files)
            {
                file_selector.AddLine(new List<string>() { file });
            }
            Window.ActivateElement(file_selector);
            string response = files[file_selector.GetResponse().Value];
            return response;
        }

        public static void Home()
        {
            ScrollingMenu home_menu = new("Select an action", choices: [
                "Display an image",
                "Filter an image",
                "Rotate an image",
                "Resize an image",
                "Generate a fractale",
                ]);
            Dialog Save = new(new List<string> { "Save the image ?"},"NO","YES");
            Text text = new(new List<string>() {""});
            TableSelector file_selector = new(headers: ["image"]);
            TableSelector filter_selector = new(headers: ["filter"], lines: new List<List<string>>() {  });
            Window.AddElement(home_menu);
            Window.AddElement(text);
            Window.AddElement(filter_selector);
            Window.AddElement(file_selector);
            Window.AddElement(Save);

            static void SaveImage(MyImage image,Dialog Save)
            {
                Window.ActivateElement(Save);
                Save.GetResponse();
            }

            string file_name = "";
            while (true)
            {
                Window.ActivateElement(home_menu);
                var responseHomeMenu = home_menu.GetResponse();
                switch (responseHomeMenu?.Status)
                {
                    case Status.Selected:
                        switch (responseHomeMenu.Value)
                        {
                            case 0:
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Display an image" });
                                Window.ActivateElement(text);
                                file_name = choose_file(file_selector);
                                //Process.Start(racine + "Images/Save/"+file_name); must be fixed
                                Window.DeactivateElement(text);
                                break;
                            case 1:
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Filter an image, select one" });
                                Window.ActivateElement(text);
                                Window.ActivateElement(filter_selector);
                                string filter = filters[filter_selector.GetResponse().Value];
                                file_name = choose_file(file_selector);
                                MyImage image = new MyImage(racine + "Images/Save/" + file_name);
                                image.filter(filter);
                                Window.DeactivateElement(text);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Status.Escaped:
                    case Status.Deleted:
                        Window.Close();
                        break;
                }
            }
        }


    }
}
