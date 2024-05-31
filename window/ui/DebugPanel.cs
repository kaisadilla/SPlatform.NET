using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.window.ui;
internal class DebugPanel {
    private RectangleShape _panelBg = new();
    private Font _font;
    private Text _infoFps = new();
    private Text _infoTime = new();
    private Text _infoTimeScale = new();

    public DebugPanel () {
        _font = new("res/fonts/CascadiaMono.ttf");
        SetupElements();
    }

    public void DrawPanel (RenderWindow window) {
        _infoTime.DisplayedString = $"Time: {Time.CurrentTime}";
        _infoTimeScale.DisplayedString = $"Time scale: {Time.TimeScale} " +
            (Time.Paused ? "[paused]" : "");

        window.Draw(_panelBg);
        window.Draw(_infoFps);
        window.Draw(_infoTime);
        window.Draw(_infoTimeScale);
    }

    private void SetupElements () {
        _panelBg.FillColor = new(0, 0, 0, 196);
        _panelBg.Position = new(5f, 3f);
        _panelBg.Size = new(220f, 42f);

        _infoFps.Font = _font;
        _infoFps.DisplayedString = $"FPS: 0";
        _infoFps.Position = new(10f, 4f);
        _infoFps.CharacterSize = 12;
        _infoFps.FillColor = Color.White;
        _infoFps.Style = Text.Styles.Regular;

        _infoTime.Font = _font;
        _infoTime.DisplayedString = "Time: 0.00";
        _infoTime.Position = new(10f, 16f);
        _infoTime.CharacterSize = 12;
        _infoTime.FillColor = Color.White;
        _infoTime.Style = Text.Styles.Regular;

        _infoTimeScale.Font = _font;
        _infoTimeScale.DisplayedString = "Time scale: 0.00 [paused]";
        _infoTimeScale.Position = new(10f, 28f);
        _infoTimeScale.CharacterSize = 12;
        _infoTimeScale.FillColor = Color.White;
        _infoTimeScale.Style = Text.Styles.Regular;
    }
}
