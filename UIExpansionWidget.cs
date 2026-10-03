using System;
using System.Xml.Linq;
using Engine;

namespace Game;

public sealed class UIExpansionWidget : CanvasWidget
{
    private readonly ComponentPlayer m_player;
    private readonly LabelWidget m_time;
    private readonly LabelWidget m_position;
    private readonly CanvasWidget m_panel;

    public UIExpansionWidget(ComponentPlayer player)
    {
        m_player = player;

        LoadContents(
            this,
            ContentManager.Get<XElement>("Widgets/UIExpansionWidget")
        );

        m_panel = Children.Find<CanvasWidget>("Panel");
        m_time = Children.Find<LabelWidget>("Time");
        m_position = Children.Find<LabelWidget>("Position");
    }

    public override void Update()
    {
        base.Update();

        ModSettingsManager.TryGet(
            out bool hud,
            UIExpansionModLoader.PackageName,
            "UIExpansionSettings",
            "HUD"
        );

        ModSettingsManager.TryGet(
            out bool worldInfo,
            UIExpansionModLoader.PackageName,
            "UIExpansionSettings",
            "WorldInfo"
        );

        m_panel.IsVisible = hud;

        if (!hud)
            return;

        m_time.Text = $"Time: {DateTime.Now:HH:mm:ss}";

        if (worldInfo)
        {
            ComponentBody? body =
                m_player.Entity.FindComponent<ComponentBody>(true);

            if (body != null)
            {
                Vector3 p = body.Position;

                m_position.Text =
                    $"X: {p.X:0}  Y: {p.Y:0}  Z: {p.Z:0}";
            }
            else
            {
                m_position.Text =
                    "X: --  Y: --  Z: --";
            }

            m_position.IsVisible = true;
        }
        else
        {
            m_position.IsVisible = false;
        }
    }
}
