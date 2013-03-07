using System;
using System.Web.Mvc;
using ibuddylib;

namespace SharePirates.BuddyWeb.Controllers
{
    public class HomeController : Controller
    {
        private static BuddyAnimator _animator;

        public HomeController()
        {
            if (_animator == null)
            {
                _animator = new BuddyAnimator(BuddyManager.Global.AnyBuddy); 

               
            }
        }

        public ActionResult Index()
        {
            return new EmptyResult();
        }

        public ActionResult Flash(string color)
        {
            _animator.FlashColor((HeadColor) Enum.Parse(typeof(HeadColor), color, true));
            return new EmptyResult();
        }
        
        public ActionResult Twist()
        {
            _animator.Twist();
            return new EmptyResult();
        }  
        public ActionResult HeartBeat(int ms=-1)
        {
            if (ms > 0)
            {
                _animator.HeartBeat(ms);
            }
            else
            {
                _animator.HeartBeat();

            }

            return new EmptyResult();
        }  
        
        public ActionResult HeadColor(string color)
        {
            _animator.Buddy.HeadColor = ((HeadColor) Enum.Parse(typeof(HeadColor), color, true));
            return new EmptyResult();
        }   
        
        public ActionResult InvertHeart()
        {
            _animator.InvertHeart();
            return new EmptyResult();
        }

        public ActionResult HeartColor(string color)
        {
            _animator.Buddy.HeartLight = ((HeartLight)Enum.Parse(typeof(HeartLight), color, true));
            return new EmptyResult();
        }

        public ActionResult Wag()
        {
            _animator.Wag();
            return new EmptyResult();
        }  
        
        public ActionResult Flap()
        {
            _animator.Flap();
            return new EmptyResult();
        }
            
    }
}
