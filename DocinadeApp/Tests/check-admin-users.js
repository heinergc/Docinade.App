// Script para verificar la funcionalidad de creación de usuarios admin
// Ejecutar con: node check-admin-users.js

const https = require('https');
const http = require('http');

// Configurar para ignorar certificados SSL auto-firmados
process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;

async function checkAdminUsersEndpoint() {
    console.log('🔍 Verificando endpoint de creación de usuarios admin...');
    
    const options = {
        hostname: 'localhost',
        port: 18163,
        path: '/Admin/Users/Create',
        method: 'GET',
        headers: {
            'User-Agent': 'Node.js Test Script',
            'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8'
        },
        rejectUnauthorized: false // Ignorar certificados SSL
    };

    return new Promise((resolve, reject) => {
        const req = https.request(options, (res) => {
            console.log(`📊 Status Code: ${res.statusCode}`);
            console.log(`📋 Headers:`, res.headers);
            
            let body = '';
            res.on('data', (chunk) => {
                body += chunk;
            });
            
            res.on('end', () => {
                if (res.statusCode === 200) {
                    console.log('✅ El endpoint está respondiendo correctamente');
                    
                    // Verificar que el formulario contenga los campos esperados
                    const hasEmailField = body.includes('asp-for="Email"') || body.includes('name="Email"');
                    const hasPasswordField = body.includes('asp-for="Password"') || body.includes('name="Password"');
                    const hasNombreField = body.includes('asp-for="Nombre"') || body.includes('name="Nombre"');
                    const hasApellidoField = body.includes('asp-for="Apellido"') || body.includes('name="Apellido"');
                    
                    console.log(`📝 Campos del formulario encontrados:`);
                    console.log(`   - Email: ${hasEmailField ? '✅' : '❌'}`);
                    console.log(`   - Contraseña: ${hasPasswordField ? '✅' : '❌'}`);
                    console.log(`   - Nombre: ${hasNombreField ? '✅' : '❌'}`);
                    console.log(`   - Apellido: ${hasApellidoField ? '✅' : '❌'}`);
                    
                    if (body.includes('Login') || body.includes('login')) {
                        console.log('⚠️  Parece que se está redirigiendo a la página de login');
                        console.log('   Esto indica que se requiere autenticación para acceder al panel admin');
                    }
                    
                } else if (res.statusCode === 302 || res.statusCode === 401 || res.statusCode === 403) {
                    console.log('🔒 Redirección detectada - se requiere autenticación');
                    console.log('   Esto es esperado para el panel de administración');
                } else {
                    console.log(`❌ Error: Status Code ${res.statusCode}`);
                }
                
                resolve({
                    statusCode: res.statusCode,
                    body: body.substring(0, 500) // Solo primeros 500 caracteres
                });
            });
        });
        
        req.on('error', (error) => {
            if (error.code === 'ECONNREFUSED') {
                console.log('❌ No se pudo conectar al servidor en https://localhost:18163');
                console.log('   Asegúrate de que la aplicación esté ejecutándose');
            } else {
                console.log('❌ Error de conexión:', error.message);
            }
            reject(error);
        });
        
        req.setTimeout(10000, () => {
            console.log('⏱️  Timeout - el servidor no respondió en 10 segundos');
            req.destroy();
            reject(new Error('Timeout'));
        });
        
        req.end();
    });
}

async function checkHealthEndpoint() {
    console.log('🏥 Verificando si el servidor está ejecutándose...');
    
    const options = {
        hostname: 'localhost',
        port: 18163,
        path: '/',
        method: 'GET',
        headers: {
            'User-Agent': 'Node.js Health Check',
        },
        rejectUnauthorized: false
    };

    return new Promise((resolve, reject) => {
        const req = https.request(options, (res) => {
            console.log(`💓 Servidor respondiendo en puerto 18163 - Status: ${res.statusCode}`);
            resolve(res.statusCode);
        });
        
        req.on('error', (error) => {
            if (error.code === 'ECONNREFUSED') {
                console.log('❌ Servidor no está ejecutándose en https://localhost:18163');
            } else {
                console.log('❌ Error de conexión:', error.message);
            }
            reject(error);
        });
        
        req.setTimeout(5000, () => {
            req.destroy();
            reject(new Error('Timeout'));
        });
        
        req.end();
    });
}

async function main() {
    console.log('🚀 Iniciando verificación del sistema de usuarios admin...\n');
    
    try {
        // Verificar que el servidor esté ejecutándose
        await checkHealthEndpoint();
        console.log('');
        
        // Verificar el endpoint de creación de usuarios
        await checkAdminUsersEndpoint();
        
        console.log('\n📋 Resumen:');
        console.log('   El endpoint /Admin/Users/Create está configurado y responde');
        console.log('   Para probar completamente, necesitas:');
        console.log('   1. Autenticarte como administrador');
        console.log('   2. Navegar a https://localhost:18163/Admin/Users/Create');
        console.log('   3. Completar el formulario y verificar que se crea el usuario');
        
    } catch (error) {
        console.log('\n❌ Error durante la verificación:', error.message);
        console.log('\n🔧 Posibles soluciones:');
        console.log('   1. Asegúrate de que la aplicación esté ejecutándose');
        console.log('   2. Ejecuta: dotnet run --urls https://localhost:18163');
        console.log('   3. Verifica que no haya errores de compilación');
    }
}

main().catch(console.error);
