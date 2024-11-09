using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI
{
    public static class Hyperlink {
        public static string Create(string text, Uri uri, Color? color = null) {
            if (!color.HasValue) {
                color = Theme.Instance.PrimaryColor;
            }
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color.Value)}><u><link=\"{uri}\">{text}</link></u></color>";
        }

        public static void Handle(GameObject g, PointerEventData e) {
            var component = g.GetComponent<TMP_Text>();
            var canvas = component.canvas;
            var camera = canvas.worldCamera;
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(component, e.position, camera);
            if (linkIndex != -1) {
                // Link was clicked, get the link ID
                TMP_LinkInfo linkInfo = component.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();
                Application.OpenURL(linkID);
            }
        }
    }
}
