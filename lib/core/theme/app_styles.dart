import 'package:flutter/material.dart';

abstract class AppStyles {
  // العناوين الكبيرة
  static const TextStyle font24Bold = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    fontFamily: 'Cairo',
    color: Color(0xFF1E293B),
  );

  static const TextStyle font20Bold = TextStyle(
    fontSize: 20,
    fontWeight: FontWeight.w700,
    fontFamily: 'Cairo',
    color: Color(0xFF1E293B),
  );

  // النصوص التوضيحية والعادية
  static const TextStyle font16Medium = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.w500,
    fontFamily: 'Cairo',
    color: Color(0xFF334155),
  );

  static const TextStyle font14Regular = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w400,
    fontFamily: 'Cairo',
    color: Color(0xFF64748B),
  );

  static const TextStyle font14Medium = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w500,
    fontFamily: 'Cairo',
    color: Color(0xFF334155),
  );

  static const TextStyle font14BoldWhite = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.bold,
    fontFamily: 'Cairo',
    color: Colors.white,
  );

  // الأزرار والنصوص الصغيرة
  static const TextStyle font12Regular = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.w400,
    fontFamily: 'Cairo',
    color: Color(0xFF94A3B8),
  );
}
