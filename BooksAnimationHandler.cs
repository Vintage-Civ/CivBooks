using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

//TODO: ALL of it /./

namespace CivBooks
{
    internal class BooksAnimationHandler : BooksAnimationHandlerBase
    {
        public ICoreClientAPI capi;

        private const float
            AnimOpenSpeed = 1.6F,
            AnimCloseSpeed = 10F;

        private const string
            AnimOpen = "bookopen",
            AnimOpenCode = "bookopening",
            AnimClose = "bookclose",
            AnimCloseCode = "bookclosing";

        private AnimationMetaData AnimMetaDataOpen = new AnimationMetaData() { Animation = AnimOpen, Code = AnimOpenCode, AnimationSpeed = AnimOpenSpeed };
        private AnimationMetaData AnimMetaDataClose = new AnimationMetaData() { Animation = AnimClose, Code = AnimCloseCode, AnimationSpeed = AnimCloseSpeed };

        BlockEntityBooks beBooks;

        AssetLocation openSound;
        AssetLocation closeSound;

        public BooksAnimationHandler(ICoreAPI api, BlockEntityBooks beBooks)
        {
            this.beBooks = beBooks;
            if (api is ICoreClientAPI capi)
            {
                this.capi = capi;
                float rotY = beBooks.Block.Shape.rotateY;
                beBooks.animUtil?.InitializeAnimator("book", new Vec3f(0, rotY, 0));
                openSound = new AssetLocation("books:sounds/effect/pageturn");
                closeSound = new AssetLocation("books:sounds/effect/closebook");
            }
        }

        public void Open()
        {
            capi.World.PlaySoundAt(openSound, beBooks.Pos.X, beBooks.Pos.Y, beBooks.Pos.Z);
            beBooks.animUtil?.StopAnimation(AnimOpenCode);
            beBooks.animUtil?.StopAnimation(AnimCloseCode);
            beBooks.animUtil?.StartAnimation(AnimMetaDataOpen);
        }

        public void Close()
        {
            capi.World.PlaySoundAt(closeSound, beBooks.Pos.X, beBooks.Pos.Y, beBooks.Pos.Z);
            beBooks.animUtil?.StopAnimation(AnimOpenCode);
            beBooks.animUtil?.StopAnimation(AnimCloseCode);
            beBooks.animUtil?.StartAnimation(AnimMetaDataClose);
        }

        public override void Dispose() => beBooks.animUtil?.Dispose();

        public bool hideDrawModel { get => HideDrawModel(); }
        private bool HideDrawModel()
        {
            if ((beBooks.animUtil?.activeAnimationsByAnimCode.Count ?? 0) > 0)
            {
                return true;
            }
            return false;
        }
    }
}