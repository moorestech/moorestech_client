using MainGame.UnityView.UI.Builder.BluePrint;

namespace MainGame.UnityView.UI.Builder.Element
{
    public interface IUIBluePrintElement
    {
        public UIBluePrintElementType ElementElementType { get; }
        public int Priority { get; }
        
        /// <summary>
        /// そのUI要素の名前
        /// 必須ではないが、そのUI要素を特定するために使う
        /// </summary>
        public string IdName { get; }
    }
}