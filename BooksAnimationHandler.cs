using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

//TODO: ALL of it /./

namespace CivBooks
{
    internal class BooksAnimationHandler : BooksAnimationHandlerBase
    {
        public ICoreClientAPI Capi;

        private static float
            AnimOpenSpeed = 0.8F,
            AnimCloseSpeed = 1F;

        private static string
            AnimatorOpen = "animbooksopen",
            AnimatorClose = "booksclose",
            AnimOpen = "bookopen",
            AnimOpenCode = "bookopening",
            AnimClose = "bookclose",
            AnimCloseCode = "bookclosing";

        private AnimationMetaData AnimMetaDataOpen = new AnimationMetaData() { Animation = AnimOpen, Code = AnimOpenCode, AnimationSpeed = AnimOpenSpeed };
        private AnimationMetaData AnimMetaDataClose = new AnimationMetaData() { Animation = AnimClose, Code = AnimCloseCode, AnimationSpeed = AnimCloseSpeed };

        private BlockEntityAnimationUtil animUtilopen, animUtilclose;

        public BooksAnimationHandler(ICoreAPI api, BlockEntityBooks BE)
        {
            if (api is ICoreClientAPI)
            {
                this.Capi = (ICoreClientAPI)api;
                animUtilopen = new BlockEntityAnimationUtil(Capi, BE);
                animUtilclose = new BlockEntityAnimationUtil(Capi, BE);
                animUtilopen.InitializeAnimator(AnimatorOpen);
                animUtilclose.InitializeAnimator(AnimatorClose);

                // new Vec3f(Block.Shape.rotateX, Block.Shape.rotateY, Block.Shape.rotateZ);
            }
        }

        public void Open()
        {
            animUtilopen.StartAnimation(AnimMetaDataOpen);
        }

        public void Open(ICoreAPI api)
        {
            if (api.World is ICoreClientAPI)
            {
                animUtilclose.InitializeAnimator(AnimatorOpen);
                animUtilclose.StartAnimation(AnimMetaDataClose);
                //animUtilopen.InitializeAnimator(AnimatorOpen);
                //animUtilopen.StartAnimation(AnimMetaDataOpen);
            }
        }

        public void Close()
        {
            animUtilopen.StopAnimation(AnimatorOpen);
            //animUtilclose.InitializeAnimator(AnimatorOpen);
        }

        public void Close(ICoreAPI api)
        {
            if (api.World is ICoreClientAPI)
            {
                animUtilopen.StopAnimation(AnimatorOpen);
                animUtilopen.activeAnimationsByAnimCode.Clear();
                animUtilopen.InitializeAnimator(AnimatorOpen);
            }
        }

        public override void Dispose() => animUtilopen.Dispose();

        public bool HideDrawModel()
        {
            if (animUtilclose.activeAnimationsByAnimCode.Count > 0)
            {
                return true;
            }

            if (animUtilopen.activeAnimationsByAnimCode.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}