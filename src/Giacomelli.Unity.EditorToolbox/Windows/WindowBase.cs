﻿using System;
using System.Text;
using Giacomelli.Unity.Metadata.Infrastructure.Bootstrap;
using UnityEditor;
using UnityEngine;

namespace Giacomelli.Unity.EditorToolbox
{
    public abstract class WindowBase : EditorWindow
    {
        #region Fields
        private Vector2 m_logScroll;
        private StringBuilder m_log = new StringBuilder();
		private Confirm m_confirm;
        #endregion

        #region Constructors
        protected WindowBase(string title, float width, float height)
        {
            titleContent.text = title;
            titleContent.tooltip = "Giacomelli Editor Toolbox / {0}".With(title);
            autoRepaintOnSceneChange = true;
            minSize = new Vector2(width, height);

            MetadataBootstrap.Setup();
        }
        #endregion

        #region GUI
        protected void ResetLog()
        {
            m_log = new StringBuilder();
            Repaint();
        }

        protected void Log(string message, params object[] args)
        {
            m_log.AppendFormat(message, args);
            m_log.AppendLine();
            Repaint();
        }

        protected void CreateLogView(float height)
        {
            m_logScroll = EditorGUILayout.BeginScrollView(m_logScroll, GUILayout.Height(height));

            var logText = m_log.ToString();
            var textHeight = GUIStyle.none.CalcHeight(new GUIContent(logText), minSize.x);
            textHeight = textHeight < height ? height : textHeight;
            EditorGUILayout.TextArea(logText, GUILayout.Height(textHeight));
            EditorGUILayout.EndScrollView();
        }

        protected void CreateControl(string label, Action createControl)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("{0}: ".With(label));
            createControl();
            EditorGUILayout.EndHorizontal();
        }

        protected void CreateHelpBox(string helpText, string url = null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(helpText, MessageType.None, false);

            if (!string.IsNullOrEmpty(url) && GUILayout.Button("Get", GUILayout.Width(40)))
            {
                Application.OpenURL(url);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

		protected abstract void PerformOnGUI();

		private void OnGUI()
		{
			if (m_confirm == null)
			{
				PerformOnGUI();
			}
			else
			{
				m_confirm.OnGUI();
			}
		}

		protected void Confirm(string message, Action yesCallback, Action noCallback = null)
		{
			yesCallback += () => m_confirm = null;
			noCallback += () => m_confirm = null;
			m_confirm = new Confirm(message, yesCallback, noCallback);
		}
        #endregion
    }
}