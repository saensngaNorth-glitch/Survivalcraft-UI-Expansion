    private void UpdateDebugHud()
    {
        if (m_debugButton != null && m_debugButton.IsClicked)
            m_debugVisible = !m_debugVisible;

        m_debugPanel.IsVisible = m_debugVisible;
        if (!m_debugVisible)
            return;

        ComponentBody? body = m_player.Entity.FindComponent<ComponentBody>(true);
        if (body == null)
            return;

        Vector3 p = body.Position;
        
        var debugPosLabel = Children.Find<LabelWidget>("DebugPosition");
        if (debugPosLabel != null)
        {
            debugPosLabel.Text = $"XYZ: {p.X:0.00} / {p.Y:0.00} / {p.Z:0.00}";
        }
    }
}
