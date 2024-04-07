// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function checkFiles() {
    var privateKeyInput = document.getElementById('privateKeyInput');
    var PrivateKey = document.getElementById('PrivateKey');
    var loadKeysButton = document.getElementById('loadKeysButton');

    if (privateKeyInput.files.length > 0 && PrivateKey.files.length > 0) {
        loadKeysButton.disabled = false;
    } else {
        loadKeysButton.disabled = true;
    }
}

var privateKeyDownloaded = false;
var publicKeyDownloaded = false;
/// <summary>
/// Downloads a file with the given text and filename
/// </summary>
/// <param name="filename">The name of the file</param>
/// <param name="text">The text to write in the file</param>
/// <param name="isPrivateKey">True if the file is a private key, false if it is a public key</param>
function download(filename, text, isPrivateKey) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);

    if (isPrivateKey) {
        privateKeyDownloaded = true;
    } else {
        publicKeyDownloaded = true;
    }

    if (privateKeyDownloaded && publicKeyDownloaded) {
        document.getElementById('continueButton').disabled = false;
    }
}