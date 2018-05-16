using System.Management.Automation;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace PSModule
{
    [Cmdlet(VerbsCommon.Get, "KeyVaultSecret")]
    [OutputType(typeof(SecretBundle))]
    public class GetKeyVaultSecretCmdlet : Cmdlet
    {
        private KeyVaultClient _keyVaultClient;

        // Position 0 allows to run the command without specifying parameters: `Get-KeyVaultSecret <secret>`
        // ValueFromPipeline allows to pipe this parameter through pipe: `$secretIdentifier | Get-KeyVaultSecret`
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public string Identifier { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// This method is invoked once, to allow the cmdlet initialize all of its dependencies
        /// </summary>
        protected override void BeginProcessing()
        {
            AzureServiceTokenProvider authentication = new AzureServiceTokenProvider();
            KeyVaultClient.AuthenticationCallback authenticationCallback =
                new KeyVaultClient.AuthenticationCallback(authentication.KeyVaultTokenCallback);

            _keyVaultClient = new KeyVaultClient(authenticationCallback);
        }

        /// <inheritdoc />
        /// <summary>
        /// This method is invoked per each "Record". Here should be the pure logic of what the cmdlet does.
        /// Note that in order to return a value, you should use the base class `Write[Type]` methods.
        /// </summary>
        protected override void ProcessRecord()
        {
            SecretBundle secret = _keyVaultClient.GetSecretAsync(Identifier).Result;

            WriteObject(secret);
        }

        /// <inheritdoc />
        /// <summary>
        /// This method is invoked after all "Records" have been processed.
        /// This method allows you to take care of resource clean up before the cmdlet ends.
        /// </summary>
        protected override void EndProcessing()
        {
            _keyVaultClient.Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        /// Invoked in case of hard stop (CTRL+C for example)
        /// </summary>
        protected override void StopProcessing()
        {
            _keyVaultClient.Dispose();
        }
    }
}