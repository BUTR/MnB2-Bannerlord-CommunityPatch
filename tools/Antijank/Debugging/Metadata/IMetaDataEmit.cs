using System;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  [Guid("BA3FEE4C-ECB9-4e41-83B7-183FA41CD859"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataEmit {

    int SetModuleProps([MarshalAs(UnmanagedType.LPWStr)] string szName);

    int Save([MarshalAs(UnmanagedType.LPWStr)] string szFile, uint dwSaveFlags);

    int SaveToStream([MarshalAs(UnmanagedType.Interface)] object pIStream, uint dwSaveFlags);

    int GetSaveSize(CorSaveSize fSave, out uint pdwSaveSize);

    int DefineTypeDef([MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements, out uint ptd);

    int DefineNestedType([MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements, uint tdEncloser, out uint ptd);

    int SetHandler([MarshalAs(UnmanagedType.IUnknown)] object pUnk);

    int DefineMethod(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwMethodFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSigBlob, uint cbSigBlob, uint ulCodeRVA, uint dwImplFlags, out uint pmd);

    int DefineMethodImpl(uint td, uint tkBody, uint tkDecl);

    int DefineTypeRefByName(uint tkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] string szName, out uint ptr);

    int DefineImportType([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, uint tdImport, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, out uint ptr);

    int DefineMemberRef(uint tkImport, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvSigBlob, uint cbSigBlob, out uint pmr);

    int DefineImportMember([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, uint mbMember, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, uint tkParent, out uint pmr);

    int DefineEvent(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szEvent, uint dwEventFlags, uint tkEventType, uint mdAddOn, uint mdRemoveOn, uint mdFire, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods, out uint pmdEvent);

    int SetClassLayout(uint td, uint dwPackSize, [MarshalAs(UnmanagedType.LPArray)] long[] rFieldOffsets, uint ulClassSize);

    int DeleteClassLayout(uint td);

    int SetFieldMarshal(uint tk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pvNativeType, uint cbNativeType);

    int DeleteFieldMarshal(uint tk);

    int DefinePermissionSet(uint tk, uint dwAction, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvPermission, uint cbPermission, out uint ppm);

    int SetRVA(uint md, uint ulRVA);

    int GetTokenFromSig([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pvSig, uint cbSig, out uint pmsig);

    int DefineModuleRef([MarshalAs(UnmanagedType.LPWStr)] string szName, out uint pmur);

    int SetParent(uint mr, uint tk);

    int GetTokenFromTypeSpec([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pvSig, uint cbSig, out uint ptypespec);

    int SaveToMemory(IntPtr pbData, uint cbData);

    int DefineUserString([MarshalAs(UnmanagedType.LPWStr)] string szString, uint cchString, out uint pstk);

    int DeleteToken(uint tkObj);

    int SetMethodProps(uint md, uint dwMethodFlags, uint ulCodeRVA, uint dwImplFlags);

    int SetTypeDefProps(uint td, uint dwTypeDefFlags, uint tkExtends, [MarshalAs(UnmanagedType.LPArray)] uint[] rtkImplements);

    int SetEventProps(uint ev, uint dwEventFlags, uint tkEventType, uint mdAddOn, uint mdRemoveOn, uint mdFire, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods);

    int SetPermissionSetProps(uint tk, uint dwAction, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pvPermission, uint cbPermission, out uint ppm);

    int DefinePinvokeMap(uint tk, uint dwMappingFlags, [MarshalAs(UnmanagedType.LPWStr)] string szImportName, uint mrImportDLL);

    int SetPinvokeMap(uint tk, uint dwMappingFlags, [MarshalAs(UnmanagedType.LPWStr)] string szImportName, uint mrImportDLL);

    int DeletePinvokeMap(uint tk);

    int DefineCustomAttribute(uint tkObj, uint tkType, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
      byte[] pCustomAttribute, uint cbCustomAttribute, out uint pcv);

    int SetCustomAttributeValue(uint pcv, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pCustomAttribute, uint cbCustomAttribute);

    int DefineField(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwFieldFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSigBlob, uint cbSigBlob, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)]
      byte[] pValue, uint cchValue, out uint pmd);

    int DefineProperty(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szProperty, uint dwPropFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pvSig, uint cbSig, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)]
      byte[] cvalue, uint cchValue, uint mdSetter, uint mdGetter, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods, out uint pmdProp);

    int DefineParam(uint md, uint ulParamSeq, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwParamFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      byte[] pValue, uint cchValue, out uint ppd);

    int SetFieldProps(uint fd, uint dwFieldFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pValue, uint cchValue);

    int SetPropertyProps(uint pr, uint dwPropFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pValue, uint cchValue, uint mdSetter, uint mdGetter, [MarshalAs(UnmanagedType.LPArray)] uint[] rmdOtherMethods);

    int SetParamProps(uint pd, [MarshalAs(UnmanagedType.LPWStr)] string szName, uint dwParamFlags, uint dwCPlusTypeFlag, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      byte[] pValue, uint cchValue);

    int DefineSecurityAttributeSet(uint tkObj, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      COR_SECATTR[] rSecAttrs, uint cSecAttrs, out uint pulErrorAttr);

    int ApplyEditAndContinue([MarshalAs(UnmanagedType.IUnknown)] object pImport);

    int TranslateSigWithScope([MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyImport pAssemImport, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, [MarshalAs(UnmanagedType.Interface)] IMetaDataImport import, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      byte[] pbSigBlob, uint cbSigBlob, [MarshalAs(UnmanagedType.Interface)] IMetaDataAssemblyEmit pAssemEmit, [MarshalAs(UnmanagedType.Interface)] IMetaDataEmit emit, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)]
      byte[] pvTranslatedSig, uint cbTranslatedSigMax, out uint pcbTranslatedSig);

    int SetMethodImplFlags(uint md, uint dwImplFlags);

    int SetFieldRVA(uint fd, uint ulRVA);

    int Merge([MarshalAs(UnmanagedType.Interface)] IMetaDataImport pImport, [MarshalAs(UnmanagedType.Interface)] IMapToken pHostMapToken, [MarshalAs(UnmanagedType.IUnknown)] object pHandler);

    int MergeEnd();

  };

}