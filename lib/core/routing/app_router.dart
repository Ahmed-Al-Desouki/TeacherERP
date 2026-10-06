import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../di/service_locator.dart';
import '../utils/session_manager.dart';

abstract class AppRouter {
  // 1. مسارات المصادقة والبداية (Auth & Onboarding)
  static const kSplash = '/';
  static const kOnboarding = '/onboarding';
  static const kLogin = '/login';
  static const kRegisterStudent = '/register-student';
  static const kVerifyOtp = '/verify-otp';

  // 2. بوابات الدخول حسب الصلاحيات (Role-Based Gates)
  static const kStudentHome = '/student-home';
  static const kAssistantHome = '/assistant-home';
  static const kTeacherDashboard = '/teacher-dashboard';

  static final GlobalKey<NavigatorState> navigatorKey =
      GlobalKey<NavigatorState>();

  static final router = GoRouter(
    navigatorKey: navigatorKey,
    initialLocation: kSplash,
    routes: [
      GoRoute(
        path: kSplash,
        builder: (context, state) => const Scaffold(
          body: Center(child: CircularProgressIndicator()),
        ), // سنستبدلها بـ SplashView
      ),
      GoRoute(
        path: kOnboarding,
        builder: (context, state) =>
            const Scaffold(body: Center(child: Text("Onboarding View"))),
      ),
      GoRoute(
        path: kLogin,
        builder: (context, state) {
          // يمكن استقبال إيميل ممرر من شاشة الـ OTP إذا وجد
          final email = state.extra as String?;
          return Scaffold(
            body: Center(child: Text("Login View - Email: $email")),
          );
        },
      ),
      GoRoute(
        path: kRegisterStudent,
        builder: (context, state) =>
            const Scaffold(body: Center(child: Text("Register Student View"))),
      ),
      GoRoute(
        path: kVerifyOtp,
        builder: (context, state) {
          final email = state.extra as String? ?? '';
          return Scaffold(body: Center(child: Text("Verify OTP for: $email")));
        },
      ),

      // الشاشات الرئيسية المؤقتة حسب الدور
      GoRoute(
        path: kStudentHome,
        builder: (context, state) =>
            const Scaffold(body: Center(child: Text("Student Home View"))),
      ),
      GoRoute(
        path: kAssistantHome,
        builder: (context, state) =>
            const Scaffold(body: Center(child: Text("Assistant Home View"))),
      ),
      GoRoute(
        path: kTeacherDashboard,
        builder: (context, state) => const Scaffold(
          body: Center(child: Text("Teacher Dashboard (Web/Tablet)")),
        ),
      ),
    ],
  );

  /// دالة مساعدة لتوجيه المستخدم بعد تسجيل الدخول حسب دوره في التوكن
  static void routeUserByRole(BuildContext context) {
    final role = getIt<SessionManager>().userRole.toLowerCase();

    if (role == 'teacher' || role == 'superadmin') {
      context.go(kTeacherDashboard);
    } else if (role == 'assistant') {
      context.go(kAssistantHome);
    } else {
      context.go(kStudentHome);
    }
  }
}
