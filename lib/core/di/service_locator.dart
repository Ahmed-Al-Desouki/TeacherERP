import 'package:get_it/get_it.dart';
import 'package:smart_teacher_erp/features/auth/data/datasources/auth_remote_data_source.dart';
import 'package:smart_teacher_erp/features/auth/data/repositories/auth_repository_impl.dart';
import 'package:smart_teacher_erp/features/auth/domain/repositories/auth_repository.dart';
import 'package:smart_teacher_erp/features/auth/presentation/controllers/auth_cubit/auth_cubit.dart';
import '../networking/api_service.dart';
import '../utils/session_manager.dart';

final getIt = GetIt.instance;

Future<void> setupServiceLocator() async {
  // 1. Networking
  getIt.registerLazySingleton<ApiService>(() => ApiService());

  // 2. Session Manager
  getIt.registerLazySingleton<SessionManager>(() => SessionManager());

  // Auth Data Source
  getIt.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(getIt<ApiService>()),
  );

  // Auth Repository
  getIt.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(
      getIt<AuthRemoteDataSource>(),
      getIt<SessionManager>(),
    ),
  );

  getIt.registerFactory<AuthCubit>(() => AuthCubit(getIt<AuthRepository>()));
}
