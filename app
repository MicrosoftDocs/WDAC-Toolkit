<?xml version="1.0" encoding="utf-8"?>
<SiPolicy xmlns="urn:schemas-microsoft-com:sipolicy">
  <VersionEx>10.0.0.0</VersionEx>
  <PlatformID>{2E07F7E4-194C-4D20-B7C9-6F44A6C5A234}</PlatformID>
  <Rules>
    <Rule>
      <Option>Enabled:Unsigned System Integrity Policy</Option>
    </Rule>
    <Rule>
      <Option>Enabled:Audit Mode</Option>
    </Rule>
    <Rule>
      <Option>Enabled:Advanced Boot Options Menu</Option>
    </Rule>
    <Rule>
      <Option>Required:Enforce Store Applications</Option>
    </Rule>
  </Rules>
  <!--EKUS-->
  <EKUs />
  <!--File Rules-->
  <FileRules />
  <!--Signers-->
  <Signers>
    <Signer ID="ID_SIGNER_S_27" Name="DigiCert SHA2 Assured ID Code Signing CA">
      <CertRoot Type="TBS" Value="E767799478F64A34B3F53FF3BB9057FE1768F4AB178041B0DCC0FF1E210CBA65" />
      <CertPublisher Value="Anaconda, Inc." />
    </Signer>
    <Signer ID="ID_SIGNER_S_28" Name="Microsoft Windows Verification PCA">
      <CertRoot Type="TBS" Value="265E5C02BDC19AA5394C2C3041FC2BD59774F918" />
      <CertPublisher Value="Microsoft Windows" />
    </Signer>
    <Signer ID="ID_SIGNER_S_29" Name="Microsoft Windows Production PCA 2011">
      <CertRoot Type="TBS" Value="4E80BE107C860DE896384B3EFF50504DC2D76AC7151DF3102A4450637A032146" />
      <CertPublisher Value="Microsoft Windows" />
    </Signer>
    <Signer ID="ID_SIGNER_S_2A" Name="DigiCert SHA2 Assured ID Code Signing CA">
      <CertRoot Type="TBS" Value="E767799478F64A34B3F53FF3BB9057FE1768F4AB178041B0DCC0FF1E210CBA65" />
      <CertPublisher Value="Notepad++" />
    </Signer>
    <Signer ID="ID_SIGNER_S_2B" Name="Microsoft Code Signing PCA 2010">
      <CertRoot Type="TBS" Value="121AF4B922A74247EA49DF50DE37609CC1451A1FE06B2CB7E1E079B492BD8195" />
      <CertPublisher Value="Microsoft Corporation" />
    </Signer>
    <Signer ID="ID_SIGNER_S_2C" Name="C2RBootStrapperData">
      <CertRoot Type="TBS" Value="E43274AB23ACC5669152BFBEBFE7FDFC1FEE4851" />
      <CertPublisher Value="C2RBootStrapperData" />
    </Signer>
    <Signer ID="ID_SIGNER_S_2D" Name="Microsoft Code Signing PCA 2011">
      <CertRoot Type="TBS" Value="F6F717A43AD9ABDDC8CEFDDE1C505462535E7D1307E630F9544A2D14FE8BF26E" />
      <CertPublisher Value="Microsoft Corporation" />
    </Signer>
    <Signer ID="ID_SIGNER_S_31" Name="DigiCert SHA2 Assured ID Code Signing CA">
      <CertRoot Type="TBS" Value="E767799478F64A34B3F53FF3BB9057FE1768F4AB178041B0DCC0FF1E210CBA65" />
      <CertPublisher Value="Spotify AB" />
    </Signer>
    <Signer ID="ID_SIGNER_S_32" Name="DigiCert SHA2 Assured ID Code Signing CA">
      <CertRoot Type="TBS" Value="E767799478F64A34B3F53FF3BB9057FE1768F4AB178041B0DCC0FF1E210CBA65" />
      <CertPublisher Value="Valve" />
    </Signer>
    <Signer ID="ID_SIGNER_S_37" Name="Microsoft Code Signing PCA 2011">
      <CertRoot Type="TBS" Value="F6F717A43AD9ABDDC8CEFDDE1C505462535E7D1307E630F9544A2D14FE8BF26E" />
      <CertPublisher Value="Microsoft Corporation" />
    </Signer>
  </Signers>
  <!--Driver Signing Scenarios-->
  <SigningScenarios>
    <SigningScenario Value="131" ID="ID_SIGNINGSCENARIO_DRIVERS_1" FriendlyName="Auto generated policy on 01-31-2020">
      <ProductSigners>
        <DeniedSigners>
          <DeniedSigner SignerId="ID_SIGNER_S_37" />
        </DeniedSigners>
      </ProductSigners>
    </SigningScenario>
    <SigningScenario Value="12" ID="ID_SIGNINGSCENARIO_WINDOWS" FriendlyName="Auto generated policy on 01-31-2020">
      <ProductSigners>
        <DeniedSigners>
          <DeniedSigner SignerId="ID_SIGNER_S_27" />
          <DeniedSigner SignerId="ID_SIGNER_S_28" />
          <DeniedSigner SignerId="ID_SIGNER_S_29" />
          <DeniedSigner SignerId="ID_SIGNER_S_2A" />
          <DeniedSigner SignerId="ID_SIGNER_S_2B" />
          <DeniedSigner SignerId="ID_SIGNER_S_2C" />
          <DeniedSigner SignerId="ID_SIGNER_S_2D" />
          <DeniedSigner SignerId="ID_SIGNER_S_31" />
          <DeniedSigner SignerId="ID_SIGNER_S_32" />
        </DeniedSigners>
      </ProductSigners>
    </SigningScenario>
  </SigningScenarios>
  <UpdatePolicySigners />
  <CiSigners>
    <CiSigner SignerId="ID_SIGNER_S_27" />
    <CiSigner SignerId="ID_SIGNER_S_28" />
    <CiSigner SignerId="ID_SIGNER_S_29" />
    <CiSigner SignerId="ID_SIGNER_S_2A" />
    <CiSigner SignerId="ID_SIGNER_S_2B" />
    <CiSigner SignerId="ID_SIGNER_S_2C" />
    <CiSigner SignerId="ID_SIGNER_S_2D" />
    <CiSigner SignerId="ID_SIGNER_S_31" />
    <CiSigner SignerId="ID_SIGNER_S_32" />
  </CiSigners>
  <HvciOptions>0</HvciOptions>
  <PolicyTypeID>{A244370E-44C9-4C06-B551-F6016E563076}</PolicyTypeID>
</SiPolicy>