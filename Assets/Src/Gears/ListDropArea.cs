using UnityEngine;
/*
public class ListDropArea : DropArea {
    public bool ExtendOnPreviousSibling = true;
    protected override void Update() {
        var myIndex = transform.GetSiblingIndex();
        //Debug.Log($"ListDropArea Update {myIndex}");
        if (ExtendOnPreviousSibling && myIndex > 0 && transform.parent) {
            var previous = transform.parent.GetChild(myIndex - 1) as RectTransform;
            var rectTransform = (CatchGraphic.transform as RectTransform);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, previous.sizeDelta.y);
        }
        base.Update();
    }
}*/

