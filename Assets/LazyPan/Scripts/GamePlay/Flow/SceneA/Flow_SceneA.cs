using UnityEngine;

namespace LazyPan {
    public class Flow_SceneA : Flow {
		private Comp UI_SceneA;

		private Entity Obj_Camera_Camera;
		private Entity Obj_Event_SceneAUI;
		private Entity Obj_Event_UserInfo;
		private Entity Obj_Event_MainInfo;
		private Entity Obj_Event_LinMou;
		private Entity Obj_Event_XiuXing;
		private Entity Obj_Event_XuShi;
		private Entity Obj_Event_LinLang;
		private Entity Obj_Event_BoYi;

        public override void Init(Flow baseFlow) {
            base.Init(baseFlow);
            ConsoleEx.Instance.ContentSave("flow", "Flow_SceneA  StartSceneA流程");
			UI_SceneA = UI.Instance.Open("UI_SceneA");

			Obj_Camera_Camera = Obj.Instance.LoadEntity("Obj_Camera_Camera");
			Obj_Event_SceneAUI = Obj.Instance.LoadEntity("Obj_Event_SceneAUI");

        }

		/*获取UI*/
		public Comp GetUI() {
			return UI_SceneA;
		}

		/*登录*/
		public void Login() {
			Obj.Instance.UnLoadEntity(Obj_Event_BoYi);
			Obj.Instance.UnLoadEntity(Obj_Event_LinLang);
			Obj.Instance.UnLoadEntity(Obj_Event_XuShi);
			Obj.Instance.UnLoadEntity(Obj_Event_XiuXing);
			Obj.Instance.UnLoadEntity(Obj_Event_LinMou);
			Obj.Instance.UnLoadEntity(Obj_Event_MainInfo);
			Obj_Event_UserInfo = Obj.Instance.LoadEntity("Obj_Event_UserInfo");
		}

		/*主面板*/
		public void Main() {
			Obj.Instance.UnLoadEntity(Obj_Event_UserInfo);
			Obj_Event_MainInfo = Obj.Instance.LoadEntity("Obj_Event_MainInfo");
		}

		/*灵谋信息*/
		public void LinMou() {
			Obj.Instance.UnLoadEntity(Obj_Event_LinMou);
			Obj_Event_LinMou = Obj.Instance.LoadEntity("Obj_Event_LinMou");
		}

		/*修行信息*/
		public void XiuXing() {
			Obj.Instance.UnLoadEntity(Obj_Event_XiuXing);
			Obj_Event_XiuXing = Obj.Instance.LoadEntity("Obj_Event_XiuXing");
		}

		/*蓄势信息*/
		public void XuShi() {
			Obj.Instance.UnLoadEntity(Obj_Event_XuShi);
			Obj_Event_XuShi = Obj.Instance.LoadEntity("Obj_Event_XuShi");
		}

		/*琳琅信息*/
		public void LinLang() {
			Obj.Instance.UnLoadEntity(Obj_Event_LinLang);
			Obj_Event_LinLang = Obj.Instance.LoadEntity("Obj_Event_LinLang");
		}

		/*博弈信息*/
		public void BoYi() {
			Obj.Instance.UnLoadEntity(Obj_Event_BoYi);
			Obj_Event_BoYi = Obj.Instance.LoadEntity("Obj_Event_BoYi");
		}


        /*下一步*/
        public void Next(string teleportSceneSign) {
            Clear();
            Launch.instance.StageLoad(teleportSceneSign);
        }

        public override void Clear() {
            base.Clear();
			Obj.Instance.UnLoadEntity(Obj_Event_LinMou);
			Obj.Instance.UnLoadEntity(Obj_Event_XiuXing);
			Obj.Instance.UnLoadEntity(Obj_Event_XuShi);
			Obj.Instance.UnLoadEntity(Obj_Event_LinLang);
			Obj.Instance.UnLoadEntity(Obj_Event_BoYi);
			Obj.Instance.UnLoadEntity(Obj_Event_UserInfo);
			Obj.Instance.UnLoadEntity(Obj_Event_MainInfo);
			Obj.Instance.UnLoadEntity(Obj_Event_SceneAUI);
			Obj.Instance.UnLoadEntity(Obj_Camera_Camera);

			UI.Instance.Close("UI_SceneA");

        }
    }
}