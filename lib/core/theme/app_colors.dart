import 'package:flutter/material.dart';

abstract class AppColors {
  // اللون الأساسي (Primary) - أزرق داكن احترافي للأنظمة التعليمية
  static const Color primary = Color(0xFF1E40AF);
  static const Color primaryDark = Color(0xFF1E3A8A);
  static const Color primaryLight = Color(0xFF3B82F6);

  // الخلفيات والأسطح
  static const Color scaffoldBackground = Color(0xFFF8FAFC);
  static const Color surface = Colors.white;

  // النصوص والحياديات
  static const Color textPrimary = Color(0xFF0F172A);
  static const Color textSecondary = Color(0xFF64748B);
  static const Color textMuted = Color(0xFF94A3B8);

  // الحدود والحقول
  static const Color border = Color(0xFFE2E8F0);
  static const Color focusedBorder = Color(0xFF2563EB);
  static const Color inputFill = Color(0xFFF8FAFC);

  // الحالات
  static const Color error = Color(0xFFDC2626);
  static const Color success = Color(0xFF16A34A);
  static const Color warning = Color(0xFFD97706);
}
