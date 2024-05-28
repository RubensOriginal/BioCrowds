using System.IO;
using UnityEngine;

namespace Biocrowds.Core.Utils
{
    public class Screenshot
    {
        void CamCapture(Camera camera, string name)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;
 
            camera.Render();
 
            Texture2D Image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
            Image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            Image.Apply();
            RenderTexture.active = currentRT;
 
            var Bytes = Image.EncodeToPNG();
            // Destroy(Image);
            
            // File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + name + "/"+ FileCounter + ".png", Bytes);
            // FileCounter++;
        }
    }
}