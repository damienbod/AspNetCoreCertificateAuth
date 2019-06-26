# Certs

CD C:\git\AspNetCoreCertificateAuth\Certs

## Create Root CA


New-SelfSignedCertificate -DnsName "root_ca_dev_damienbod.com", "root_ca_dev_damienbod.com" -CertStoreLocation "cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears(20) `

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText

Get-ChildItem -Path cert:\localMachine\my\"The thumbprint..." | Export-PfxCertificate -FilePath C:\git\root_ca_dev_damienbod.pfx -Password $mypwd

Export-Certificate -Cert cert:\localMachine\my\"The thumbprint..." -FilePath root_ca_dev_damienbod.crt

## Create Child Cert from Root

$rootcert = ( Get-ChildItem -Path cert:\LocalMachine\My\"The thumbprint..." )

New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname "child_a_dev_damienbod.com" -Signer $rootcert

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText

Get-ChildItem -Path cert:\localMachine\my\"The thumbprint..." | Export-PfxCertificate -FilePath C:\git\AspNetCoreCertificateAuth\Certs\child_a_dev_damienbod.pfx -Password $mypwd

Export-Certificate -Cert cert:\localMachine\my\"The thumbprint..." -FilePath child_a_dev_damienbod.crt

## Example:

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText

Get-ChildItem -Path cert:\localMachine\my\991B2B416D7D403F0BA4B972F43F75B9726EDA61 | Export-PfxCertificate -FilePath C:\git\AspNetCoreCertificateAuth\Certs\root_ca_dev_damienbod.pfx -Password $mypwd

Export-Certificate -Cert cert:\localMachine\my\991B2B416D7D403F0BA4B972F43F75B9726EDA61 -FilePath C:\git\AspNetCoreCertificateAuth\Certs\root_ca_dev_damienbod.crt

-------

$rootcert = ( Get-ChildItem -Path cert:\LocalMachine\My\991B2B416D7D403F0BA4B972F43F75B9726EDA61 )

New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname "child_a_dev_damienbod.com" -Signer $rootcert

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText

Get-ChildItem -Path cert:\localMachine\my\8C5C2F90325B2FFBFDB82A69073C5EC7A3E771B9 | Export-PfxCertificate -FilePath C:\git\AspNetCoreCertificateAuth\Certs\child_a_dev_damienbod.pfx -Password $mypwd

Export-Certificate -Cert cert:\localMachine\my\8C5C2F90325B2FFBFDB82A69073C5EC7A3E771B9 -FilePath child_a_dev_damienbod.crt

-------

