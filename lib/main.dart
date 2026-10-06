import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:smart_teacher_erp/l10n/app_localizations.dart';
import 'core/di/service_locator.dart';
import 'core/routing/app_router.dart';
import 'core/theme/app_theme.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await setupServiceLocator();

  runApp(const SmartTeacherApp());
}

class SmartTeacherApp extends StatelessWidget {
  const SmartTeacherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Smart Teacher ERP',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      routerConfig: AppRouter.router,

      // إعدادات اللغات والاتجاهات
      locale: const Locale('ar'), // اللغة العربية كلغة افتراضية
      localizationsDelegates: const [
        AppLocalizations.delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      supportedLocales: AppLocalizations.supportedLocales,
    );
  }
}
