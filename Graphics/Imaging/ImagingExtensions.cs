﻿// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ImagingExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    public static class ImagingExtensions {

        public static Color GetAverageColor( [NotNull] this Bitmap bitmap ) {
            if ( bitmap is null ) {
                throw new ArgumentNullException( nameof( bitmap ) );
            }

            var red = 0;
            var green = 0;
            var blue = 0;

            using ( var faster = new FasterBitmap( bitmap ) ) {
                for ( var x = 0; x < bitmap.Width; x++ ) {
                    for ( var y = 0; y < bitmap.Height; y++ ) {
                        var pixel = faster.GetPixel( x, y );
                        red += pixel.R;
                        green += pixel.G;
                        blue += pixel.B;
                    }
                }
            }

            var total = bitmap.Width * bitmap.Height;

            red /= total;
            green /= total;
            blue /= total;

            return Color.FromArgb( red, green, blue );
        }
    }
}