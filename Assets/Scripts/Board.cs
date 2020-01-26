using System;
using UnityEngine;

namespace MainGame.Board
{
    abstract class Board
    {
        GameObject gameObject;
        Animation  animation;

        public Board(GameObject gameObject, Animation animation)
        {
            this.gameObject = gameObject;
            this.animation  = animation;
        }

        public abstract void PlayAnimation();
    }

    class MainBoard : Board
    {
        public MainBoard(GameObject gameObject, Animation animation) : base(gameObject, animation)
        {

        }

        public override void PlayAnimation()
        {

        }
    }

    class WarningBoard : Board
    {
        public WarningBoard(GameObject gameObject, Animation animation) : base(gameObject, animation)
        {

        }

        public override void PlayAnimation()
        {

        }
    }
}