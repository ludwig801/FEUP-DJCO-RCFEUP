using UnityEngine;
using System.Collections.Generic;

public class CamerasManager : MonoBehaviour
{
    public List<ChaseCam> Cameras;

    public void FitCameras()
    {
        var numPlayers = Cameras.Count;
        var w = numPlayers <= 2 ? 1 : 0.5f;
        var h = numPlayers == 1 ? 1 : 0.5f;

        for (int i = 0; i < Cameras.Count; i++)
        {
            var chaseCam = Cameras[i];
            var halfOffset = (i % 2) * 0.5f;
            var x = numPlayers <= 2 ? 0 : halfOffset;
            var y = numPlayers == 1 ? 0 : numPlayers == 2 ? (((i + 1) % 2) * 0.5f) : 1 - (0.25f + (i / 2) * 0.5f);

            var camRect = new Rect();
            camRect.position = new Vector2(x, y);
            camRect.size = new Vector2(w, h);
            chaseCam.Cam.rect = camRect;
        }
    }
}
