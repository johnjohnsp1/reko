// ls.h
// Generated by decompiling ls
// using Reko decompiler version 0.6.1.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals" (4028C0 Eq_28 t4028C0) (411E60 Eq_31 t411E60) (411ED0 Eq_32 t411ED0) (619FF8 word64 qw619FF8))
	globals_t (in globals : (ptr (struct "Globals")))
Eq_17: (fn void ())
	T_17 (in rdx : (ptr Eq_17))
	T_33 (in rtld_fini : (ptr (fn void ())))
Eq_20: (fn void (ptr64))
	T_20 (in __align : ptr64)
Eq_26: (fn int32 ((ptr Eq_28), Eq_29, (ptr (ptr char)), (ptr Eq_31), (ptr Eq_32), (ptr Eq_17), (ptr void)))
	T_26 (in __libc_start_main : ptr64)
	T_27 (in signature of __libc_start_main : void)
Eq_28: (fn int32 (int32, (ptr (ptr char)), (ptr (ptr char))))
	T_28 (in main : (ptr (fn int32 (int32, (ptr (ptr char)), (ptr (ptr char))))))
	T_35 (in 0x00000000004028C0 : word64)
Eq_29: (union "Eq_29" (int32 u0) (word64 u1))
	T_29 (in argc : int32)
	T_36 (in qwArg00 : word64)
Eq_31: (fn void ())
	T_31 (in init : (ptr (fn void ())))
	T_38 (in 0x0000000000411E60 : word64)
Eq_32: (fn void ())
	T_32 (in fini : (ptr (fn void ())))
	T_39 (in 0x0000000000411ED0 : word64)
Eq_45: (fn void ())
	T_45 (in __hlt : ptr64)
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr Eq_1)
  OrigDataType: (ptr (struct "Globals"))
T_2: (in rax_4 : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_3: (in 0000000000619FF8 : ptr64)
  Class: Eq_3
  DataType: (ptr word64)
  OrigDataType: (ptr (struct (0 T_6 t0000)))
T_4: (in 0x0000000000000000 : word64)
  Class: Eq_4
  DataType: word64
  OrigDataType: word64
T_5: (in 0x0000000000619FF8 + 0x0000000000000000 : word64)
  Class: Eq_5
  DataType: ptr64
  OrigDataType: ptr64
T_6: (in Mem0[0x0000000000619FF8 + 0x0000000000000000:word64] : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_7: (in 0x0000000000000000 : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_8: (in rax_4 == 0x0000000000000000 : bool)
  Class: Eq_8
  DataType: bool
  OrigDataType: bool
T_9: (in rsp_15 : word64)
  Class: Eq_9
  DataType: word64
  OrigDataType: word64
T_10: (in SCZO_16 : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_11: (in rax_17 : word64)
  Class: Eq_11
  DataType: word64
  OrigDataType: word64
T_12: (in SZO_18 : byte)
  Class: Eq_12
  DataType: byte
  OrigDataType: byte
T_13: (in C_19 : byte)
  Class: Eq_13
  DataType: byte
  OrigDataType: byte
T_14: (in Z_20 : byte)
  Class: Eq_14
  DataType: byte
  OrigDataType: byte
T_15: (in __gmon_start__ : ptr64)
  Class: Eq_15
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_16: (in rax : word64)
  Class: Eq_16
  DataType: word64
  OrigDataType: word64
T_17: (in rdx : (ptr Eq_17))
  Class: Eq_17
  DataType: (ptr Eq_17)
  OrigDataType: (ptr (fn void ()))
T_18: (in qwArg00 : word64)
  Class: Eq_18
  DataType: word64
  OrigDataType: word64
T_19: (in dwArg04 : word32)
  Class: Eq_19
  DataType: word32
  OrigDataType: word32
T_20: (in __align : ptr64)
  Class: Eq_20
  DataType: (ptr Eq_20)
  OrigDataType: (ptr (fn T_24 (T_23)))
T_21: (in fp : ptr64)
  Class: Eq_21
  DataType: ptr64
  OrigDataType: ptr64
T_22: (in 0x0000000000000008 : word64)
  Class: Eq_22
  DataType: int64
  OrigDataType: int64
T_23: (in fp + 0x0000000000000008 : word64)
  Class: Eq_23
  DataType: ptr64
  OrigDataType: ptr64
T_24: (in __align(fp + 0x0000000000000008) : void)
  Class: Eq_24
  DataType: void
  OrigDataType: void
T_25: (in rax_21 : word64)
  Class: Eq_25
  DataType: word64
  OrigDataType: word64
T_26: (in __libc_start_main : ptr64)
  Class: Eq_26
  DataType: (ptr Eq_26)
  OrigDataType: (ptr (fn T_43 (T_35, T_36, T_37, T_38, T_39, T_17, T_42)))
T_27: (in signature of __libc_start_main : void)
  Class: Eq_26
  DataType: (ptr Eq_26)
  OrigDataType: 
T_28: (in main : (ptr (fn int32 (int32, (ptr (ptr char)), (ptr (ptr char))))))
  Class: Eq_28
  DataType: (ptr Eq_28)
  OrigDataType: 
T_29: (in argc : int32)
  Class: Eq_29
  DataType: Eq_29
  OrigDataType: 
T_30: (in ubp_av : (ptr (ptr char)))
  Class: Eq_30
  DataType: (ptr (ptr char))
  OrigDataType: 
T_31: (in init : (ptr (fn void ())))
  Class: Eq_31
  DataType: (ptr Eq_31)
  OrigDataType: 
T_32: (in fini : (ptr (fn void ())))
  Class: Eq_32
  DataType: (ptr Eq_32)
  OrigDataType: 
T_33: (in rtld_fini : (ptr (fn void ())))
  Class: Eq_17
  DataType: (ptr Eq_17)
  OrigDataType: 
T_34: (in stack_end : (ptr void))
  Class: Eq_34
  DataType: (ptr void)
  OrigDataType: 
T_35: (in 0x00000000004028C0 : word64)
  Class: Eq_28
  DataType: (ptr Eq_28)
  OrigDataType: (ptr (fn int32 (int32, (ptr (ptr char)), (ptr (ptr char)))))
T_36: (in qwArg00 : word64)
  Class: Eq_29
  DataType: Eq_29
  OrigDataType: (union (int32 u1) (word64 u0))
T_37: (in fp + 0x0000000000000008 : word64)
  Class: Eq_30
  DataType: (ptr (ptr char))
  OrigDataType: (ptr (ptr char))
T_38: (in 0x0000000000411E60 : word64)
  Class: Eq_31
  DataType: (ptr Eq_31)
  OrigDataType: (ptr (fn void ()))
T_39: (in 0x0000000000411ED0 : word64)
  Class: Eq_32
  DataType: (ptr Eq_32)
  OrigDataType: (ptr (fn void ()))
T_40: (in 0x0000000000000004 : word64)
  Class: Eq_40
  DataType: int64
  OrigDataType: int64
T_41: (in fp + 0x0000000000000004 : word64)
  Class: Eq_41
  DataType: ptr64
  OrigDataType: ptr64
T_42: (in DPB(qwArg00, fp + 0x0000000000000004, 0) : word64)
  Class: Eq_34
  DataType: (ptr void)
  OrigDataType: (ptr void)
T_43: (in __libc_start_main(&globals->t4028C0, qwArg00, fp + 0x0000000000000008, &globals->t411E60, &globals->t411ED0, rdx, DPB(qwArg00, fp + 0x0000000000000004, 0)) : int32)
  Class: Eq_43
  DataType: int32
  OrigDataType: int32
T_44: (in DPB(rax, __libc_start_main(&globals->t4028C0, qwArg00, fp + 0x0000000000000008, &globals->t411E60, &globals->t411ED0, rdx, DPB(qwArg00, fp + 0x0000000000000004, 0)), 0) : word64)
  Class: Eq_25
  DataType: word64
  OrigDataType: word64
T_45: (in __hlt : ptr64)
  Class: Eq_45
  DataType: (ptr Eq_45)
  OrigDataType: (ptr (fn T_46 ()))
T_46: (in __hlt() : void)
  Class: Eq_46
  DataType: void
  OrigDataType: void
*/
typedef struct Globals {
	Eq_28 t4028C0;	// 4028C0
	Eq_31 t411E60;	// 411E60
	Eq_32 t411ED0;	// 411ED0
	word64 qw619FF8;	// 619FF8
} Eq_1;

typedef void (Eq_17)();

typedef void (Eq_20)(ptr64);

typedef int32 (Eq_26)( *, Eq_29, char * *,  *,  *,  *, void);

typedef int32 (Eq_28)(int32 rdi, char * * rsi, char * * rdx);

typedef union Eq_29 {
	int32 u0;
	word64 u1;
} Eq_29;

typedef void (Eq_31)();

typedef void (Eq_32)();

typedef void (Eq_45)();

