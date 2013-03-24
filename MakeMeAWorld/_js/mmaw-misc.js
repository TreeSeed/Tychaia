"use strict";

/// <summary>
/// Miscellanous functionality for Make Me a World.
/// </summary>
function MMAWMisc()
{
    /// <summary>
    /// Calculates a numeric representation based on
    /// an arbitrary string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A numeric hash of the string.</returns>
    this.calculateNumberFromInput = function(input) {
        var i = 0;
        for (var a = 0; a < input.length; a += 1) {
            i += (a * a * a) * input.charCodeAt(a);
        }
        return i;
    };
    
    /// <summary>
    /// Hashes or returns directly a numeric value for
    /// the input.  If the input is a number, it is
    /// returned directly, otherwise it is hashed with
    /// calculateNumberFromInput.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A numeric value for the input.</param>
    this.returnNumberOrHash = function(input) {
        if (isNaN(input) == true) {
            return this.calculateNumberFromInput(input);
        } else {
            return input;
        }
    };
    
    /// <summary>
    /// Pads out a number with zeros.
    /// </summary>
    this.zeroFill = function(number, width) {
        width -= number.toString().length;
        if (width > 0) {
            return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number;
        }
        return number + ""; // always return a string
    }
};
