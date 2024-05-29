using Retromono.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.animation;
internal class TweenAnimation<T> {
    private bool _animating = false;
    private bool _loop = false;
    private ITween _tween;
    private Action? endCallback;


}
